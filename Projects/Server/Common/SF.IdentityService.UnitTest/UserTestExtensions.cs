using SF.Auth.IdentityServices;
using System;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.Sys.Data;
using SF.Sys.Clients;
using SF.Sys.UnitTest;
using SF.Sys;

namespace SF.IdentityService.UnitTest
{
	public class UserInfo
	{
		public User User { get; set; }
		public string Account { get; set; }
		public string Password { get; set; }
	}
	public static class UserTestExtensions
	{
		//public static async Task<T> UserTest<T>(this IServiceProvider sp,Func<IUserService, Task<T>> Action)
		//{
		//	return await sp.TestManagedService(
		//		sim => { return null; },
		//		Action,
		//		async (isp, sim, svc) =>
		//		{
		//			await sim.NewDataProtectorService().Ensure(isp, svc);
		//			await sim.NewPasswordHasherService().Ensure(isp, svc);
		//		});
		//}
		public static async Task<long> UserSignin(
			this IServiceProvider sp,
			string account = null,
			string password = "123456",
			bool returnToken=false,
			string client= "app.android"
			)
		{
			var svc = sp.Resolve<IUserService>();
			var signinResult = await svc.Signin(new SigninArgument
			{
				Ident = account,
				Password = password,
				Mode = returnToken?SigninMode.AccessToken: SigninMode.Cookie,
				ClientId = client
			});
			if (returnToken)
			{
				var uid = await svc.ValidateAccessToken(signinResult);
				return uid;
			}
			else
			{
				var uid=await svc.GetCurUserId();
				return uid.Value;
			}
		}

		public static async Task<string> UserBindCredical(
			this IServiceProvider sp,
			string Credical="phone",
			string Value=null
			)
		{
			var us = sp.Resolve<IUserService>();
			var cr = await us.GetUserCredential(Credical);
			Assert.IsTrue(string.IsNullOrEmpty(cr.Value));

			var ig = sp.Resolve<IIdentGenerator>();
			if (Value==null)
				Value = "133" + (await ig.GenerateAsync("外部测试用户ID")).ToString().PadLeft(8, '0');

			await us.SendBindCredentialVerifyCode(
				new SendBindCredentialVerifyCodeArgument
				{
					CredentialProvider = Credical,
					Credetial = Value
				});
			await us.BindCredential(new BindCredentialArgument
			{
				VerifyCode = "000000",
				Credential = Value,
				CredentialProvider = Credical
			});
			var ncr = await us.GetUserCredential(Credical);
			Assert.AreEqual(Value,ncr.Value);

			return Value;
		}

		public static async Task<UserInfo> UserCreate(
			this IServiceProvider sp,
			string prefix=null,
			string account = null,
			string name = null,
			string entity = "测试用户",
			string password = "123456",
			bool ReturnToken = false,
			string client = "app.android",
			bool sendVerifyCode=false
		)
		{

			if (prefix == null) prefix = "";

			var svc = sp.Resolve<IUserService>();
			var ig = sp.Resolve<IIdentGenerator>();
			var postfix = await ig.GenerateAsync("测试");
			account = prefix +(account ?? "131" + postfix.ToString().PadLeft(8, '0'));
			name = prefix + (name ?? "测试用户" + postfix);
			var icon = prefix + "icon" + postfix;

			if (sendVerifyCode)
				await svc.SendCreateIdentityVerifyCode(new SendCreateIdentityVerifyCodeArgument
				{
					Credetial = account
				});

			var accessToken = await svc.Signup(
				new SignupArgument
				{
					VerifyCode = "000000",
					Credential = account,
					Password = password,
					User = new User
					{
						Id = postfix,
						//Entity = entity,
						Name = name,
						Icon = icon
					},
					ReturnToken = ReturnToken,
					ClientId= client
				}, true
				);

			User User;
			if (ReturnToken)
			{
				var uid = await svc.ValidateAccessToken(accessToken);
				Assert.AreEqual(postfix, uid);

				User = await svc.GetUser(uid);
				Assert.AreEqual(name, User.Name);
				Assert.AreEqual(icon, User.Icon);
				//Assert.AreEqual(entity, ii.Entity);
				//Assert.AreEqual(postfix, ii.Id);
			}
			else
			{
				var at = sp.Resolve<IAccessToken>();
				var uid=at.User.GetUserIdent();

				User = await svc.GetCurUser();
				Assert.AreEqual(uid.Value, User.Id);

				Assert.AreEqual(name, User.Name);
				Assert.AreEqual(icon, User.Icon);
			//	Assert.AreEqual(entity, uii.);
			}

			var uid2 = await sp.UserSignin(account, password);
			Assert.AreEqual(User.Id, uid2);
			return new UserInfo { User = User, Account = account, Password = password };
		}

		public static async Task<T> WithNewUser<T>(this IServiceProvider sp,Func<User,Task<T>> Callback)
		{
			var user=await sp.UserCreate();
			return await Callback(user.User);
		}
		public static IScope<UserInfo> WithNewSigninedUser(this IScope<IServiceProvider> scope,string prefix=null)
		{
			return from sp in scope
				   from re in UserCreate(sp, prefix)
				   select re;
		}
	}
	

}
