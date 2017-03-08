using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public class UserService: 
		IUserService
	{
		UserServiceSetting Setting { get; }
		public UserService(UserServiceSetting Setting)
		{
			this.Setting = Setting;
		}

		
		public async Task<UserInfo> GetCurUser()
		{
			var uid = await Setting.AuthSessionProvider.Value.GetCurrentUserId();
			if (uid == null)
				throw new PublicInvalidOperationException("没有登录");
			return await Setting.UserStorage.FindById(uid.Value);
		}

		public async Task Update(UserInfo User)
		{
			var uid = await Setting.AuthSessionProvider.Value.GetCurrentUserId();
			if (uid == null)
				throw new PublicInvalidOperationException("没有登录");
			if (uid.Value != User.Id)
				throw new PublicDeniedException("不能修改其他人的用户信息");

			if (string.IsNullOrWhiteSpace(User.NickName)) throw new PublicArgumentException("请输入昵称");
			if(User.NickName.Length < 2) throw new PublicArgumentException("昵称太短");
			if (User.NickName.Length > 20) throw new PublicArgumentException("昵称太长");
			await Setting.UserStorage.UpdateAsync(User);
		}



		public async Task<UserInfo> Signin(UserSigninArgument Arg)
		{
			if (string.IsNullOrWhiteSpace(Arg.Ident))
				throw new PublicArgumentException("请输入用户标识");

			if (string.IsNullOrWhiteSpace(Arg.Password))
				throw new PublicArgumentException("请输入密码");

			UserIdent ui = null;
			foreach(var ip in Setting.SigninIdentProviders.Value)
			{
				ui =await ip.Find(Arg.Ident,null);
				if (ui!=null)
					break;
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

			var user =await Setting.UserStorage.SigninSuccess(
				ui.UserId, 
				Setting.AccessInfo.Value.Value
				);
			await Setting.AuthSessionProvider.Value.BindUser(user);
			return user;
		}

		public Task Signout()
		{
			return Setting.AuthSessionProvider.Value.UnbindUser();
		}

		public async Task<string> SendSignupVerifyCode(SendSignupVerifyCodeArgument Arg)
		{
			if (string.IsNullOrWhiteSpace(Arg.Ident))
				throw new ArgumentException($"需要提供{Setting.SignupIdentProvider.Value.Name}");
			var msg = await Setting.SignupIdentProvider.Value.Verify(Arg.Ident);
			if (msg != null)
				throw new ArgumentException(msg);

			if (!await Setting.SignupIdentProvider.Value.CanSendMessage())
				throw new NotSupportedException();

			await Setting.SignupIdentProvider.Value.SendMessage(
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
		public async Task<UserInfo> Signup(UserSignupArgument Arg)
		{
			if(string.IsNullOrWhiteSpace(Arg.Ident))
				throw new ArgumentException("请输入用户标识");
			var msg = await Setting.SignupIdentProvider.Value.Verify(Arg.Ident);
			if (msg != null)
				throw new ArgumentException(msg);

			var canSendMessage = await Setting.SignupIdentProvider.Value.CanSendMessage();
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

			var nickName = Arg.NickName?.Trim();
			if (nickName == null)
				nickName = "U"+new Random().Next(1000000).ToString().PadLeft(6, '0');

			if (string.IsNullOrWhiteSpace(Arg.Password))
				throw new PublicArgumentException("请输入密码");

			var user=await Setting.UserStorage.Create(
				new UserCreateArgument
				{
					AccessInfo = Setting.AccessInfo.Value.Value,
					PasswordHash = Setting.PasswordHasher.Value.Hash(Arg.Password),
					SecurityStamp=Guid.NewGuid().ToString("N"),
					User = new UserInfo
					{
						Id=uid,
						Icon = Arg.Icon,
						Image = Arg.Image,
						NickName = nickName,
						Sex = Arg.Sex
					}
				});
			await Setting.AuthSessionProvider.Value.BindUser(user);
			return user;
		}
		
		public Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg)
		{
			throw new NotImplementedException();
		}

		public async Task ResetPasswordByRecoveryCode(ResePasswordByRecorveryCodeArgument Arg)
		{
			if (string.IsNullOrWhiteSpace(Arg.IdentProviderId))
				throw new ArgumentException("缺少用户标识类型");
			if (string.IsNullOrWhiteSpace(Arg.Ident))
				throw new PublicArgumentException("需要输入用户标识");

			//verify code 
			var ui = await Setting.SignupIdentProvider.Value.Find(Arg.Ident,null);
			if (ui == null)
				throw new PublicArgumentException("找不到指定的用户");

			await Setting.UserStorage.SetPasswordHash(
				ui.UserId, 
				Setting.PasswordHasher.Value.Hash(Arg.NewPassword),
				Guid.NewGuid().ToString("N")
				);

			var user = await Setting.UserStorage.FindById(ui.UserId);
			await Setting.AuthSessionProvider.Value.BindUser(user);
		}

		public async Task SetPassword(SetPasswordArgument Arg)
		{
			if (string.IsNullOrWhiteSpace(Arg.OldPassword))
				throw new PublicArgumentException("需要输入旧密码");
			if(string.IsNullOrWhiteSpace(Arg.NewPassword))
				throw new PublicArgumentException("需要输入新密码");

			var uid = await Setting.AuthSessionProvider.Value.GetCurrentUserId();
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

	}
}
