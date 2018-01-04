using SF.Auth.IdentityServices;
using System;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.Sys.Data;
using SF.Sys.Clients;
using SF.Sys.UnitTest;

namespace SF.IdentityService.UnitTest
{
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
				ReturnToken = returnToken,
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

		public static async Task<(User user, string account, string password)> UserCreate(
			this IServiceProvider sp,
			string account = null,
			string name = null,
			string entity = "测试用户",
			string password = "123456",
			bool ReturnToken = false,
			string client = "app.android"
		)
		{
			var svc = sp.Resolve<IUserService>();
			var ig = sp.Resolve<IIdentGenerator>();
			var postfix = await ig.GenerateAsync("测试");
			account = account ?? "131" + postfix.ToString().PadLeft(8, '0');
			name = name ?? "测试用户" + postfix;
			var icon = "icon" + postfix;

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

			User user;
			if (ReturnToken)
			{
				var uid = await svc.ValidateAccessToken(accessToken);
				Assert.AreEqual(postfix, uid);

				user = await svc.GetUser(uid);
				Assert.AreEqual(name, user.Name);
				Assert.AreEqual(icon, user.Icon);
				//Assert.AreEqual(entity, ii.Entity);
				//Assert.AreEqual(postfix, ii.Id);
			}
			else
			{
				var at = sp.Resolve<IAccessToken>();
				var uid=at.User.GetUserIdent();

				user = await svc.GetCurUser();
				Assert.AreEqual(uid.Value, user.Id);

				Assert.AreEqual(name, user.Name);
				Assert.AreEqual(icon, user.Icon);
			//	Assert.AreEqual(entity, uii.);
			}

			var uid2 = await sp.UserSignin(account, password);
			Assert.AreEqual(user.Id, uid2);
			return (user, account,password);
		}

		public static async Task<T> WithCurUser<T>(this IServiceProvider sp,Func<User,Task<T>> Callback)
		{
			var user=await sp.UserCreate();
			return await Callback(user.user);
		}
		public static ITestContext<(OV,User User,string Account,string Password)> WithSignedUser<OV>(this ITestContext<OV> scope)
		{
			return scope.NewContext<OV, (OV Prev, User User, string Account, string Password)>(
				async (sp, ov, cb) =>
				 {
					 var re = await UserCreate(sp);
					 await cb(sp, (Prev:ov, re.user, re.account, re.password));
				 });
		}
	}
	

}
