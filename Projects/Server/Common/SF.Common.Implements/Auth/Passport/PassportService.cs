using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Passport.Models;

namespace SF.Auth.Passport
{
	public class PassportService: 
		IPassportService
	{
		PassportServiceSetting Setting { get; }
		public PassportService(PassportServiceSetting Setting)
		{
			this.Setting = Setting;
		}
		
		public async Task<UserDesc> Signin(SigninArgument Arg)
		{
			var token = await Setting.SigninProvider.Value.CreateAccessToken(
				new Internals.CreateAccessTokenArgument
				{
					Ident = Arg.Ident,
					Password = Arg.Password
				});
			if (token == null)
				throw new PublicArgumentException("账号不存在或密码错误");
			return await SigninByAccessToken(token,Arg.Expires);
		}

		public async Task Signout()
		{
			await Setting.AuthSessionProvider.Value.UnbindSession();
		}

		public async Task<UserDesc> SigninByAccessToken(string AccessToken,int? Expires)
		{
			var uid = await Setting.SigninProvider.Value.ParseAccessToken(AccessToken);
			var expires = Expires.HasValue?(DateTime?)Setting.TimeService.Value.Now.AddSeconds(Expires.Value):null;
			var sid = await Setting.UserSessionStorage.Value.Create(
				uid,
				expires,
				Setting.AccessInfo.Value.Value
				);
			return await UpdateSession(
				sid, 
				uid,
				expires
				);
		}

		public async Task<UserDesc> GetCurUser()
		{
			var sess = await Setting.AuthSessionProvider.Value.GetUserSession();
			if (sess == null)
				return null;
			return sess.User;
		}

		public async Task<UserDesc> UpdateSession()
		{
			var sess = await Setting.AuthSessionProvider.Value.GetUserSession();
			if (sess == null)
				throw new InvalidOperationException("未登录");
			return await UpdateSession(sess.Id, sess.User.Id, sess.Expires);
		}
		async Task<UserDesc> UpdateSession(long SessionId,long UserId,DateTime? Expires)
		{
			var desc = await Setting.SigninProvider.Value.GetUserDesc(UserId);
			var sess = new UserSession
			{
				Id = SessionId,
				User = desc,
				Expires= Expires
			};
			await Setting.AuthSessionProvider.Value.BindSession(sess);
			return desc;
		}
	}
}
