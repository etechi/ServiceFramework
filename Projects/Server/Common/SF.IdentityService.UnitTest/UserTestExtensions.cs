using SF.Auth.IdentityServices;
using System;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.Sys.Data;

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
			bool returnToken=true
			)
		{
			var svc = sp.Resolve<IUserService>();
			var signinResult = await svc.Signin(new SigninArgument
			{
				Ident = account,
				Password = password,
				ReturnToken = returnToken
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
			long id=0,
			string account = null,
			string name=null,
			string entity="测试用户",
			string password = "123456",
			bool ReturnToken =true
		)
		{
			var svc = sp.Resolve<IUserService>();
			if (id == 0)
			{
				var ig = sp.Resolve<IIdentGenerator>();
				id = await ig.GenerateAsync("测试");
			}
			account = account ?? "131" + id.ToString().PadLeft(8,'0');
			name = name ?? "测试用户" + id;
			var icon = "icon" + id;

			await svc.SendCreateIdentityVerifyCode(new SendCreateIdentityVerifyCodeArgument
			{
				Credetial = account
			});

			var accessToken = await svc.Signup(
				new SignupArgument
				{
					VerifyCode="123456",
					Credential = account,
					Password = password,
					User = new  User
					{
						Id = id,
						//Entity = entity,
						Name = name,
						Icon= icon
					},
					ReturnToken = ReturnToken
				}, true
				);


			var ii = await svc.GetUser(id);
			Assert.AreEqual(name, ii.Name);
			Assert.AreEqual(icon, ii.Icon);
			//Assert.AreEqual(entity, ii.Entity);
			Assert.AreEqual(id, ii.Id);
			if (ReturnToken)
			{
				var uid = await svc.ValidateAccessToken(accessToken);
				Assert.AreEqual(id, uid);
			}
			else
			{
				var uii = await svc.GetCurUser();
				Assert.AreEqual(id, uii.Id);
				Assert.AreEqual(name, uii.Name);
				Assert.AreEqual(icon, uii.Icon);
			//	Assert.AreEqual(entity, uii.);
				Assert.AreEqual(id, uii.Id);
			}

			var uid2 = await sp.UserSignin(account, password);
			Assert.AreEqual(id, uid2);
			return (ii,account,password);
		}
	}
	

}
