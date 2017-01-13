using ServiceProtocol.Data.Entity;
using SF.Data;
using SF.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
			var uid = await Setting.AuthSessionProvider.GetCurrentUserId();
			if (uid == null)
				throw new PublicInvalidOperationException("没有登录");
			return await Setting.UserProvider.FindById(uid.Value);
		}

		public async Task Update(UserInfo User)
		{
			var uid = await Setting.AuthSessionProvider.GetCurrentUserId();
			if (uid == null)
				throw new PublicInvalidOperationException("没有登录");
			if (uid.Value != User.Id)
				throw new PublicDeniedException("不能修改其他人的用户信息");

			if (string.IsNullOrWhiteSpace(User.NickName)) throw new PublicArgumentException("请输入昵称");
			if(User.NickName.Length < 2) throw new PublicArgumentException("昵称太短");
			if (User.NickName.Length > 20) throw new PublicArgumentException("昵称太长");
			await Setting.UserProvider.Update(User);
		}



		public async Task<UserInfo> Signin(UserSigninArgument Arg)
		{
			if (string.IsNullOrWhiteSpace(Arg.Ident))
				throw new PublicArgumentException("请输入用户标识");

			if (string.IsNullOrWhiteSpace(Arg.Password))
				throw new PublicArgumentException("请输入密码");

			long? userId = null;
			foreach(var ip in Setting.IdentProviders)
			{
				if (ip.Disabled)
					continue;
				userId =await ip.IdentProvider.FindUserId(Arg.Ident);
				if (userId.HasValue)
					break;
			}
			if (!userId.HasValue)
				throw new PublicArgumentException("用户或密码错误！");

			var passwordHash = await Setting.UserProvider.GetPasswordHash(userId.Value,true);
			var passwordVerified = Setting.PasswordHasher.Hash(Arg.Password) == passwordHash;
			var user=await Setting.UserProvider.Signin(
				userId.Value, 
				passwordVerified, 
				Setting.AccessInfo.Value
				);
			if (user == null && passwordVerified)
				throw new PublicArgumentException("用户或密码错误!");
			await Setting.AuthSessionProvider.BindUser(user);
			return user;
		}

		public Task Signout()
		{
			return Setting.AuthSessionProvider.UnbindUser();
		}

		public async Task<UserInfo> Signup(UserSignupArgument Arg)
		{
			if ((Arg?.Idents?.Length ?? 0) == 0)
				throw new PublicArgumentException("必须提供用户标识");
			foreach (var id in Arg.Idents)
				if (!Setting.IdentProviders.Any(ip => ip.IdentProvider.Id == id.ProviderId))
					throw new ArgumentException("不支持用户标识类型:" + id.ProviderId);

			if(Setting.SignupVerifyCodeRequired)
			{
				if (string.IsNullOrWhiteSpace(Arg.VerifyCode))
					throw new PublicArgumentException("需要验证码");

			}

			var nickName = Arg.NickName?.Trim();
			if (nickName == null)
				nickName = "U"+new Random().Next(1000000).ToString().PadLeft(6, '0');

			if (string.IsNullOrWhiteSpace(Arg.Password))
				throw new PublicArgumentException("请输入密码");

			var user=await Setting.UserProvider.Create(new UserCreateArgument
			{
				AccessInfo = Setting.AccessInfo.Value,
				Password = Setting.PasswordHasher.Hash(Arg.Password),
				User = new UserInfo
				{
					Icon = Arg.Icon,
					Image = Arg.Image,
					NickName = nickName,
					Sex = Arg.Sex
				},
				Idents = Arg.Idents.Select(id => new UserIdent
				{
					ProviderId = id.ProviderId,
					Ident = id.Ident
				}).ToArray()
			});

			await Setting.AuthSessionProvider.BindUser(user);
			return user;
		}
		
		public Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg)
		{
			throw new NotImplementedException();
		}

		public async Task ResertPasswordByRecoveryCode(ResePasswordByRecorveryCodeArgument Arg)
		{
			if (string.IsNullOrWhiteSpace(Arg.IdentProviderId))
				throw new ArgumentException("缺少用户标识类型");
			if (string.IsNullOrWhiteSpace(Arg.Ident))
				throw new PublicArgumentException("需要输入用户标识");

			//verify code 
			var uid = await Setting.UserProvider.FindUserIdByIdent(Arg.IdentProviderId, Arg.Ident);
			if (uid == null)
				throw new PublicArgumentException("找不到指定的用户");

			await Setting.UserProvider.SetPasswordHash(uid.Value, Setting.PasswordHasher.Hash(Arg.NewPassword));
			var user = await Setting.UserProvider.FindById(uid.Value);
			await Setting.AuthSessionProvider.BindUser(user);
		}

		public async Task SetPassword(SetPasswordArgument Arg)
		{
			if (string.IsNullOrWhiteSpace(Arg.OldPassword))
				throw new PublicArgumentException("需要输入旧密码");
			if(string.IsNullOrWhiteSpace(Arg.NewPassword))
				throw new PublicArgumentException("需要输入新密码");

			var uid = await Setting.AuthSessionProvider.GetCurrentUserId();
			if (uid == null)
				throw new PublicInvalidOperationException("没有登录");

			var passwordHash = await Setting.UserProvider.GetPasswordHash(uid.Value, false);
			var passwordVerified = Setting.PasswordHasher.Hash(Arg.OldPassword) == passwordHash;
			if (!passwordVerified)
				throw new PublicArgumentException("旧密码错误");

			await Setting.UserProvider.SetPasswordHash(uid.Value, Setting.PasswordHasher.Hash(Arg.NewPassword));
		}
	}
}
