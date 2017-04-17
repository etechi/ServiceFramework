using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identity.Models;
using SF.Auth.Passport.Internals;
using SF.Auth.Passport.Models;
using SF.Auth.Identity.Internals;

namespace SF.Auth.Identity
{
	public class UserService: 
		IUserService,
		ISigninProvider
	{
		UserServiceSetting Setting { get; }
		public UserService(UserServiceSetting Setting)
		{
			this.Setting = Setting;
		}

		
		public async Task<UserInfo> GetCurUser()
		{
			var uid = await Setting.AuthSessionProvider.Value.GetCurUserId();
			if (uid == null)
				throw new PublicInvalidOperationException("没有登录");
			return await Setting.UserStorage.FindById(uid.Value);
		}

		public async Task Update(UserInfo User)
		{
			var uid = (await Setting.AuthSessionProvider.Value.GetUserSession())?.User?.Id;
			if (uid == null)
				throw new PublicInvalidOperationException("没有登录");
			if (uid.Value != User.Id)
				throw new PublicDeniedException("不能修改其他人的用户信息");

			if (string.IsNullOrWhiteSpace(User.NickName)) throw new PublicArgumentException("请输入昵称");
			if(User.NickName.Length < 2) throw new PublicArgumentException("昵称太短");
			if (User.NickName.Length > 20) throw new PublicArgumentException("昵称太长");
			await Setting.UserStorage.UpdateAsync(User);
		}
		
		public async Task<string> CreateAccessToken(CreateAccessTokenArgument Arg)
		{
			if (string.IsNullOrWhiteSpace(Arg.Ident))
				throw new PublicArgumentException("请输入用户标识");

			if (string.IsNullOrWhiteSpace(Arg.Password))
				throw new PublicArgumentException("请输入密码");

			IdentBind ui = null;
			IIdentBindProvider identProvider=null;
			foreach(var ip in Setting.SigninIdentProviders.Value)
			{
				ui =await ip.Find(Arg.Ident,null);
				if (ui != null)
				{
					identProvider = ip;
					break;
				}
			}
			if (ui==null)
				throw new PublicArgumentException("用户或密码错误！");

			var passwordHash = await Setting.UserStorage.GetPasswordHash(ui.UserId,true);
			if (Setting.PasswordHasher.Value.Hash(Arg.Password) != passwordHash)
			{
				await Setting.UserStorage.SigninFailed(
					ui.UserId,
					3,
					TimeSpan.FromMinutes(10),
					Setting.AccessInfo.Value.Value
					);

				throw new PublicArgumentException("用户或密码错误!");
			}

			await Setting.UserStorage.SigninSuccess(
				ui.UserId, 
				Setting.AccessInfo.Value.Value
				);

			return await CreateAccessToken(ui.UserId,passwordHash);

		}
		async Task<string> CreateAccessToken(long UserId,string PasswordHash,int Expires=6*60)
		{
			return (await Setting.DataProtector.Value.Encrypt(
				"用户访问令牌",
				BitConverter.GetBytes(UserId),
				Setting.TimeService.Value.Now.AddSeconds(Expires),
				PasswordHash.UTF8Bytes()
				)).Base64();
		}
		
		public Task Signout()
		{
			return Setting.AuthSessionProvider.Value.UnbindSession();
		}

		public async Task<string> SendSignupVerifyCode(SendSignupVerifyCodeArgument Arg)
		{
			if (string.IsNullOrWhiteSpace(Arg.Ident))
				throw new ArgumentException($"需要提供{Setting.SignupIdentProvider.Value.Name}");
			var msg = await Setting.SignupIdentProvider.Value.VerifyFormat(Arg.Ident);
			if (msg != null)
				throw new ArgumentException(msg);

			if (!await Setting.SignupIdentProvider.Value.Confirmable())
				throw new NotSupportedException();

			await Setting.SignupIdentProvider.Value.SendConfirmCode(
				Arg.Ident,
				"注册",
				"注册用户",
				null
				);

			return null;
		}
		void CheckVerifyCode(string Ident,string Code,string Purpose)
		{
			if (string.IsNullOrWhiteSpace(Code))
				throw new PublicArgumentException("请输入验证码");
			if (Code != Ident)
				throw new PublicArgumentException("验证码有误，请重新输入");
		}
		public async Task<string> Signup(UserSignupArgument Arg)
		{
			if(string.IsNullOrWhiteSpace(Arg.Ident))
				throw new ArgumentException("请输入用户标识");
			var msg = await Setting.SignupIdentProvider.Value.VerifyFormat(Arg.Ident);
			if (msg != null)
				throw new ArgumentException(msg);

			var canSendMessage = await Setting.SignupIdentProvider.Value.Confirmable();
			if (canSendMessage)
				CheckVerifyCode(Arg.Ident, Arg.VerifyCode, "User.Signup");


			var ui=await Setting.SignupIdentProvider.Value.Find(Arg.Ident,null);
			if (ui != null)
				throw new PublicArgumentException($"您输入的{Setting.SignupIdentProvider.Value.Name}已被注册");

			var uid = await Setting.IdentGenerator.Value.GenerateAsync("Sys.User");

			ui = await Setting.SignupIdentProvider.Value.FindOrBind(
				Arg.Ident, 
				null, 
				canSendMessage,
				uid
				);
			if(ui.UserId!=uid)
				throw new PublicArgumentException($"您输入的{Setting.SignupIdentProvider.Value.Name}已被注册");

			var nickName = Arg.UserInfo?.NickName?.Trim();
			if (nickName == null)
				nickName = "U"+new Random().Next(1000000).ToString().PadLeft(6, '0');

			if (string.IsNullOrWhiteSpace(Arg.Password))
				throw new PublicArgumentException("请输入密码");
			var passwordHash = Setting.PasswordHasher.Value.Hash(Arg.Password);
			await Setting.UserStorage.Create(
				new IdentCreateArgument
				{
					AccessInfo = Setting.AccessInfo.Value.Value,
					PasswordHash = passwordHash,
					SecurityStamp=Guid.NewGuid().ToString("N"),
					User = new UserInfo
					{
						Id=uid,
						Icon = Arg.UserInfo?.Icon,
						Image = Arg.UserInfo?.Image,
						NickName = nickName,
						Sex = Arg.UserInfo?.Sex
					}
				});
			return await CreateAccessToken(uid, passwordHash);
		}
		
		public Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg)
		{
			throw new NotImplementedException();
		}

		public async Task<string> ResetPasswordByRecoveryCode(ResePasswordByRecorveryCodeArgument Arg)
		{
			if (string.IsNullOrWhiteSpace(Arg.IdentProviderId))
				throw new ArgumentException("缺少用户标识类型");
			if (string.IsNullOrWhiteSpace(Arg.Ident))
				throw new PublicArgumentException("需要输入用户标识");

			//verify code 
			var ui = await Setting.SignupIdentProvider.Value.Find(Arg.Ident,null);
			if (ui == null)
				throw new PublicArgumentException("找不到指定的用户");
			var passwordHash = Setting.PasswordHasher.Value.Hash(Arg.NewPassword);
			await Setting.UserStorage.SetPasswordHash(
				ui.UserId, 
				passwordHash,
				Guid.NewGuid().ToString("N")
				);
			return await CreateAccessToken(ui.UserId, passwordHash);

		}

		public async Task SetPassword(SetPasswordArgument Arg)
		{
			if (string.IsNullOrWhiteSpace(Arg.OldPassword))
				throw new PublicArgumentException("需要输入旧密码");
			if(string.IsNullOrWhiteSpace(Arg.NewPassword))
				throw new PublicArgumentException("需要输入新密码");

			var uid = await Setting.AuthSessionProvider.Value.GetCurUserId();
			if (uid == null)
				throw new PublicInvalidOperationException("没有登录");

			var passwordHash = await Setting.UserStorage.GetPasswordHash(uid.Value, false);
			var passwordVerified = Setting.PasswordHasher.Value.Hash(Arg.OldPassword) == passwordHash;
			if (!passwordVerified)
				throw new PublicArgumentException("旧密码错误");

			await Setting.UserStorage.SetPasswordHash(
				uid.Value, 
				Setting.PasswordHasher.Value.Hash(Arg.NewPassword),
				Guid.NewGuid().ToString("N")
				);
		}

		Task<string> ISigninProvider.CreateAccessToken(Passport.Internals.CreateAccessTokenArgument Argument)
		{
			return CreateAccessToken(new CreateAccessTokenArgument
			{
				Ident = Argument.Ident,
				Password = Argument.Password,
			});
		}

		async Task<long> ISigninProvider.ParseAccessToken(string AccessToken)
		{
			if (string.IsNullOrWhiteSpace(AccessToken))
				throw new PublicArgumentException("请输入用户访问令牌");

			var uid = BitConverter.ToInt64(await Setting.DataProtector.Value.Decrypt(
				"用户访问令牌",
				AccessToken.Base64(),
				Setting.TimeService.Value.Now,
				async (bytes) =>
				{
					var tuid = BitConverter.ToInt64(bytes, 0);
					return (
						await Setting.UserStorage.GetPasswordHash(tuid, false)
					).UTF8Bytes();
				}), 0);
			return uid;
		}

		async Task<UserDesc> ISigninProvider.GetUserDesc(long UserId)
		{
			return await Setting.UserStorage.FindById(UserId);
		}
	}
}
