using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Auth.Identities.Internals;
using SF.Core.Caching;
namespace SF.Auth.Identities
{
	public class IdentityService :
		IIdentityService
	{
		IdentityServiceSetting Setting { get; }
		public IdentityService(IdentityServiceSetting Setting)
		{
			this.Setting = Setting;
		}
		Task<IdentityData> GetIdentityData(long Id)=>
			Setting.IdentityDataCache.Value.GetOrCreateAsync(
				  Id.ToString(),
				  () => Setting.IdentStorage.Value.Load(Id),
				  TimeSpan.FromHours(1)
				  );

		public async Task<Identity> GetIdentity(long Id)
		{
			var data = await GetIdentityData(Id);
			return data == null ? null : new Identity
			{
				Id = data.Id,
				Icon = data.Icon,
				Name = data.Name
			};
		}
		public async Task<long?> GetCurIdentityId()
		{
			return await ParseAccessToken(
					Setting.ClientService.Value.GetAccessToken()
				);
		}
		public async Task<Identity> GetCurIdentity()
		{
			var id = await GetCurIdentityId();
			return id.HasValue ? await GetIdentity(id.Value) : null;
		}
		public async Task<long> EnsureCurUserId()
		{
			var id = await GetCurIdentityId();
			if (id.HasValue) return id.Value;
			throw new PublicDeniedException("未登录");
		}
		async Task<byte[]> LoadAccessTokenPassword(byte[] data)
		{
			var uid = BitConverter.ToInt64(data, 0);
			var idata = await GetIdentityData(uid);
			return idata.SecurityStamp;
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


		async Task<string> SendVerifyCode(IIdentityCredentialProvider CredentialProvider,ConfirmMessageType Type, string Ident, long UserId, string BizIdent)
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
			if (Setting.VerifyCodeVisible)
				return code;

			await CredentialProvider.SendConfirmCode(
				Ident,
				code,
				Type,
				BizIdent
				);

			return null;
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
			var vc = CheckVerifyCode(ConfirmMessageType.PasswordRecorvery, Arg.Credential, 0, Arg.VerifyCode);

			var newPasswordHash = Setting.PasswordHasher.Value.Hash(Arg.NewPassword);
			var stamp = Bytes.Random(16);
			await Setting.IdentStorage.Value.UpdateSecurity(vc.UserId, newPasswordHash, stamp);
			return await SetOrReturnAccessToken(
				vc.UserId,
				null,
				Arg.ReturnToken
				);
		}

		public async Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg, long DefaultCredentialProviderId)
		{
			var provider = Setting.CredentialProviderResolver(Arg.CredentialProviderId ?? DefaultCredentialProviderId);
			var err = await provider.VerifyFormat(Arg.Credential);
			if (err != null)
				throw new PublicArgumentException(err);

			var bind = await provider.Find(Arg.Credential, null);
			if (bind == null)
				throw new PublicArgumentException("找不到账号");
			return await SendVerifyCode(
				provider,
				ConfirmMessageType.PasswordRecorvery, 
				Arg.Credential, 
				bind.UserId, 
				null
				);
		}

		public async Task<string> SetPassword(SetPasswordArgument Arg)
		{
			var uid = await EnsureCurUserId();
			var hash = (await GetIdentityData(uid)).PasswordHash;
			var oldPasswordHash = Setting.PasswordHasher.Value.Hash(Arg.OldPassword);
			if (oldPasswordHash != hash)
				throw new ArgumentException("旧密码错误");
			var newPasswordHash = Setting.PasswordHasher.Value.Hash(Arg.NewPassword);
			var stamp = Bytes.Random(16);
			await Setting.IdentStorage.Value.UpdateSecurity(uid, newPasswordHash, stamp);
			return await SetOrReturnAccessToken(
				uid,
				null,
				Arg.ReturnToken
				);
		}
		protected DateTime? GetExpires(int? Expires) =>
			Expires.HasValue ? (DateTime?)Setting.TimeService.Value.Now.AddSeconds(Expires.Value) : null;

		public async Task<string> Signin(SigninArgument Arg,long[] SigninCredentialProviderIds)
		{
			if (string.IsNullOrWhiteSpace(Arg.Credential))
				throw new PublicArgumentException("请输入用户标识");

			if (string.IsNullOrWhiteSpace(Arg.Password))
				throw new PublicArgumentException("请输入密码");


			IdentityCredential ui = null;
			IIdentityCredentialProvider identProvider = null;
			foreach (var pid in SigninCredentialProviderIds)
			{
				var ip = Setting.CredentialProviderResolver(pid);
				if (ip.VerifyFormat(Arg.Credential) != null)
					continue;
				ui = await ip.Find(Arg.Credential, null);
				if (ui != null)
				{
					identProvider = ip;
					break;
				}
			}
			if (ui == null)
				throw new PublicArgumentException("用户或密码错误！");
			var idData = await GetIdentityData(ui.UserId);
			var passwordHash = idData.PasswordHash;
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

			if (!idData.IsEnabled)
				throw new PublicDeniedException("用户已被禁止登录");

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
			var stamp = (await GetIdentityData(Id)).SecurityStamp;
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

		public async Task<string> SendCreateIdentityVerifyCode(SendCreateIdentityVerifyCodeArgument Arg,long DefaultCredentialProviderId)
		{
			var cpid = Arg.CredentialProviderId ?? DefaultCredentialProviderId;
			var CredentialProvider = Setting.CredentialProviderResolver(cpid);

			var err=await CredentialProvider.VerifyFormat(Arg.Credetial);
			if (err != null)
				throw new PublicArgumentException(err);

			return await SendVerifyCode(
				CredentialProvider,
				ConfirmMessageType.Signup,
				Arg.Credetial,
				0,
				null
				);
		}

		public async Task<string> CreateIdentity(CreateIdentityArgument Arg, bool VerifyCode, long DefaultCredentialProviderId)
		{
			if (string.IsNullOrWhiteSpace(Arg.Credential))
				throw new PublicArgumentException("请输入用户标识");
			var msg = await CredentialProvider.VerifyFormat(Arg.Credential);
			if (msg != null)
				throw new ArgumentException(msg);
			
			var canSendMessage = CredentialProvider.IsConfirmable();
			if (canSendMessage)
				CheckVerifyCode(
					ConfirmMessageType.Signup, 
					Arg.Credential, 
					0, 
					Arg.VerifyCode
					);


			var ui = await CredentialProvider.Find(Arg.Credential, null);
			if (ui != null)
				throw new PublicArgumentException($"您输入的{CredentialProvider.Name}已被注册");

			var uid = Arg.Identity.Id;

			ui = await CredentialProvider.FindOrBind(
				Arg.Credential,
				null,
				canSendMessage,
				uid
				);
			if (ui.UserId != uid)
				throw new PublicArgumentException($"您输入的{CredentialProvider.Name}已被注册");

			if (string.IsNullOrWhiteSpace(Arg.Password))
				throw new PublicArgumentException("请输入密码");
			var passwordHash = Setting.PasswordHasher.Value.Hash(Arg.Password);
			await Setting.IdentStorage.Value.Create(
				new IdentityCreateArgument
				{
					AccessSource = Setting.ClientService.Value.AccessSource,
					PasswordHash = passwordHash,
					SecurityStamp = Bytes.Random(16),
					Identity=new Identity
					{
						Id = uid,
						Entity = Arg.Identity.Entity,
						Icon = Arg.Identity.Icon,
						Name = Arg.Identity.Name
					}
				});
			return await SetOrReturnAccessToken(uid, Arg.Expires, Arg.ReturnToken);
		}

		public async Task UpdateIdentity(Identity Identity)
		{
			var cid = await GetCurIdentityId();
			if (!cid.HasValue)
				throw new PublicDeniedException("用户未登录");
			if (Identity.Id == 0)
				Identity.Id = cid.Value;
			else if (Identity.Id != cid.Value)
				throw new PublicDeniedException("禁止访问");

			await Setting.IdentStorage.Value.UpdateDescription(Identity);
		}
	}

}
