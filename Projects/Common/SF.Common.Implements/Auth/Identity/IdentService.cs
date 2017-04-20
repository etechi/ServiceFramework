using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identity.Models;
using SF.Auth.Identity.Internals;
using SF.Core.Caching;
namespace SF.Auth.Identity
{
	public class IdentService :
		IIdentService
	{
		IdentServiceSetting Setting { get; }
		public IdentService(IdentServiceSetting Setting)
		{
			this.Setting = Setting;
		}

		public Task<long?> GetCurUserId()
		{
			return ParseAccessToken(
				Setting.ClientService.Value.GetAccessToken(
					
					)
				);
		}
		public async Task<long> EnsureCurUserId()
		{
			var id = await GetCurUserId();
			if (id.HasValue) return id.Value;
			throw new PublicDeniedException("未登录");
		}
		async Task<byte[]> LoadAccessTokenPassword(byte[] data)
		{
			var uid = BitConverter.ToInt64(data, 0);
			var re1 = await Setting.AccessTokenPasswordCache.Value.GetOrCreateAsync(
				uid.ToString(),
				async () =>
					 new AccessTokenPassword
					 {
						 Password = await Setting.IdentStorage.Value.GetSecurityStamp(uid)
					 },
				TimeSpan.FromHours(1)
				);
			return re1.Password;
		}
		public async Task<long?> ParseAccessToken(string AccessToken)
		{
			var re=await Setting.DataProtector.Value.Decrypt(
				"访问令牌",
				AccessToken.Base64(),
				Setting.TimeService.Value.Now,
				LoadAccessTokenPassword
				);
			return BitConverter.ToInt64(re,0);
		}


		async Task<string> SendVerifyCode(ConfirmMessageType Type, string Ident, long UserId, string BizIdent)
		{
			var code = Strings.Numbers.Random(6);

			Setting.VerifyCodeCache.Value.Set(
				$"{ConfirmMessageType.PasswordRecorvery}\n{Ident}\n{UserId}",
				new VerifyCode
				{
					Type = ConfirmMessageType.PasswordRecorvery,
					Code = code,
					UserId = UserId
				},
				Setting.TimeService.Value.Now.AddMinutes(15)
				);

			await Setting.SignupIdentProvider.Value.SendConfirmCode(
				Setting.ClientService.Value.CurrentScopeId,
				Ident,
				code,
				Type,
				BizIdent
				);

			return Setting.VerifyCodeVisible ? code : null;
		}
		VerifyCode CheckVerifyCode(ConfirmMessageType Type,string Ident,long UserId,string Code)
		{
			var verifyCode = Setting.VerifyCodeCache.Value.Get(
				$"{Type}\n{Ident}\n{UserId}"
				);
			if (verifyCode == null)
				throw new PublicInvalidOperationException("请先发送验证码");
			if (verifyCode.Code != Code)
				throw new PublicDeniedException("验证码错误");
			return verifyCode;
		}


		public async Task<string> ResetPasswordByRecoveryCode(ResetPasswordByRecorveryCodeArgument Arg)
		{
			var vc = CheckVerifyCode(ConfirmMessageType.PasswordRecorvery, Arg.Ident, 0, Arg.VerifyCode);

			var newPasswordHash = Setting.PasswordHasher.Value.Hash(Arg.NewPassword);
			var stamp = Bytes.Random(16);
			await Setting.IdentStorage.Value.SetPasswordHash(vc.UserId, newPasswordHash, stamp);
			return await SetOrReturnAccessToken(
				vc.UserId,
				null,
				Arg.ReturnToken
				);
		}

		public async Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg)
		{
			var err = await Setting.SignupIdentProvider.Value.VerifyFormat(Arg.Ident);
			if (err != null)
				throw new PublicArgumentException(err);

			var bind = await Setting.SignupIdentProvider.Value.Find(Setting.ClientService.Value.CurrentScopeId, Arg.Ident, null);
			if (bind == null)
				throw new PublicArgumentException("找不到账号");
			return await SendVerifyCode(
				ConfirmMessageType.PasswordRecorvery, 
				Arg.Ident, 
				bind.UserId, 
				null
				);
		}

		public async Task<string> SetPassword(SetPasswordArgument Arg)
		{
			var uid = await EnsureCurUserId();
			var hash = await Setting.IdentStorage.Value.GetPasswordHash(uid);
			var oldPasswordHash = Setting.PasswordHasher.Value.Hash(Arg.OldPassword);
			if (oldPasswordHash != hash)
				throw new ArgumentException("旧密码错误");
			var newPasswordHash = Setting.PasswordHasher.Value.Hash(Arg.NewPassword);
			var stamp = Bytes.Random(16);
			await Setting.IdentStorage.Value.SetPasswordHash(uid, newPasswordHash, stamp);
			return await SetOrReturnAccessToken(
				uid,
				null,
				Arg.ReturnToken
				);
		}
		protected DateTime? GetExpires(int? Expires) =>
			Expires.HasValue ? (DateTime?)Setting.TimeService.Value.Now.AddSeconds(Expires.Value) : null;

		public async Task<string> Signin(SigninArgument Arg)
		{
			if (string.IsNullOrWhiteSpace(Arg.Ident))
				throw new PublicArgumentException("请输入用户标识");

			if (string.IsNullOrWhiteSpace(Arg.Password))
				throw new PublicArgumentException("请输入密码");


			var scopeId = Setting.ClientService.Value.CurrentScopeId;
			IdentBind ui = null;
			IIdentBindProvider identProvider = null;
			foreach (var ip in Setting.SigninIdentProviders.Value)
			{
				if (ip.VerifyFormat(Arg.Ident) != null)
					continue;
				ui = await ip.Find(scopeId,Arg.Ident, null);
				if (ui != null)
				{
					identProvider = ip;
					break;
				}
			}
			if (ui == null)
				throw new PublicArgumentException("用户或密码错误！");

			var passwordHash = await Setting.IdentStorage.Value.GetPasswordHash(ui.UserId);
			if (Setting.PasswordHasher.Value.Hash(Arg.Password) != passwordHash)
			{
				//await Setting.UserStorage.SigninFailed(
				//	ui.UserId,
				//	3,
				//	TimeSpan.FromMinutes(10),
				//	Setting.AccessInfo.Value.Value
				//	);

				throw new PublicArgumentException("用户或密码错误!");
			}

			//await Setting.UserStorage.SigninSuccess(
			//	ui.UserId,
			//	Setting.AccessInfo.Value.Value
			//	);
			return await SetOrReturnAccessToken(ui.UserId, Arg.Expires, Arg.ReturnToken);


		}

		async Task<string> SetOrReturnAccessToken(long UserId,int? Expires,bool ReturnToken)
		{
			var DateExpires = GetExpires(Expires);
			var token = await CreateAccessToken(UserId, DateExpires);
			if (ReturnToken)
				return token;
			await Setting.ClientService.Value.SetAccessToken(token);
			return null;
		}

		public async Task<string> CreateAccessToken(long Id,DateTime? Expires)
		{
			var stamp = await Setting.IdentStorage.Value.GetSecurityStamp(Id);
			var re = await Setting.DataProtector.Value.Encrypt(
				"用户访问令牌",
				BitConverter.GetBytes(Id),
				Expires ?? Setting.TimeService.Value.Now.AddYears(1),
				stamp
				);
			return re.Base64();

		}
		public Task Signout()
		{
			return Setting.ClientService.Value.SetAccessToken(null);
		}

		public async Task<string> SendCreateIdentVerifyCode(SendCreateIdentVerifyCodeArgument Arg)
		{
			var err=await Setting.SignupIdentProvider.Value.VerifyFormat(Arg.Ident);
			if (err != null)
				throw new PublicArgumentException(err);

			return await SendVerifyCode(
				ConfirmMessageType.Signup,
				Arg.Ident,
				0,
				null
				);
		}

		public async Task<string> CreateIdent(CreateIdentArgument Arg, bool VerifyCode)
		{
			if (string.IsNullOrWhiteSpace(Arg.Ident))
				throw new PublicArgumentException("请输入用户标识");
			var msg = await Setting.SignupIdentProvider.Value.VerifyFormat(Arg.Ident);
			if (msg != null)
				throw new ArgumentException(msg);
			
			var canSendMessage = await Setting.SignupIdentProvider.Value.IsConfirmable();
			if (canSendMessage)
				CheckVerifyCode(
					ConfirmMessageType.Signup, 
					Arg.Ident, 
					0, 
					Arg.VerifyCode
					);

			var scopeId = Setting.ClientService.Value.CurrentScopeId;

			var ui = await Setting.SignupIdentProvider.Value.Find(scopeId, Arg.Ident, null);
			if (ui != null)
				throw new PublicArgumentException($"您输入的{Setting.SignupIdentProvider.Value.Name}已被注册");

			var uid = await Setting.IdentGenerator.Value.GenerateAsync("Sys.User");

			ui = await Setting.SignupIdentProvider.Value.FindOrBind(
				scopeId,
				Arg.Ident,
				null,
				canSendMessage,
				uid
				);
			if (ui.UserId != uid)
				throw new PublicArgumentException($"您输入的{Setting.SignupIdentProvider.Value.Name}已被注册");

			if (string.IsNullOrWhiteSpace(Arg.Password))
				throw new PublicArgumentException("请输入密码");
			var passwordHash = Setting.PasswordHasher.Value.Hash(Arg.Password);
			await Setting.IdentStorage.Value.Create(
				new IdentCreateArgument
				{
					AccessInfo = Setting.ClientService.Value.AccessSource,
					PasswordHash = passwordHash,
					SecurityStamp = Bytes.Random(16),
					Id=uid
				});
			return await SetOrReturnAccessToken(uid, Arg.Expires, Arg.ReturnToken);
		}
	}

}
