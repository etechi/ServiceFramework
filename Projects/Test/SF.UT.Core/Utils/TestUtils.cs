using System;
using Xunit;
using SF.Core.ServiceManagement;
using System.Threading.Tasks;
using SF.Data;
using SF.Auth.Identities;
using SF.Auth.Identities.Models;

namespace SF.UT.Utils
{
	public static class IdentityTestUtils
	{
		public static async Task<T> IdentityTest<T>(this IServiceProvider sp,Func<IIdentityService, Task<T>> Action)
		{
			var ig = sp.Resolve<IIdentGenerator>();
			return await sp.TestService(
				sim => sim.NewAuthIdentityServive(),
				Action,
				async (isp, sim, svc) =>
				{
					await sim.NewDataProtectorService().Ensure(isp, svc);
					await sim.NewPasswordHasherService().Ensure(isp, svc);
				});
		}
		public static async Task<long> IdentitySignin(
			this IServiceProvider sp,
			string account = null,
			string password = "123456",
			bool returnToken=true
			)
		{
			var svc = sp.Resolve<IIdentityService>();
			var signinResult = await svc.Signin(new SigninArgument
			{
				Credential = account,
				Password = password,
				ReturnToken = returnToken
			});
			if (returnToken)
			{
				var uid = await svc.ParseAccessToken(signinResult);
				return uid;
			}
			else
			{
				var uid=await svc.GetCurIdentityId();
				return uid.Value;
			}
		}

		public static async Task<(Identity identity, string account, string password)> IdentityCreate(
			this IServiceProvider sp,
			long id=0,
			string account = null,
			string name=null,
			string entity="测试用户",
			string password = "123456",
			bool ReturnToken =true
		)
		{
			var ig = sp.Resolve<IIdentGenerator>();
			var svc = sp.Resolve<IIdentityService>();

			id = id == 0 ? await ig.GenerateAsync("测试用户") : id;
			account = account ?? "user" + id;
			name = name ?? "测试用户" + id;
			var icon = "icon" + id;
			var accessToken = await svc.CreateIdentity(
				new CreateIdentityArgument
				{
					Credential = account,
					Password = password,
					Identity = new SF.Auth.Identities.Models.Identity
					{
						Id = id,
						Entity = entity,
						Name = name,
						Icon= icon
					},
					ReturnToken = ReturnToken
				}, true
				);


			var ii = await svc.GetIdentity(id);
			Assert.Equal(name, ii.Name);
			Assert.Equal(icon, ii.Icon);
			Assert.Equal(entity, ii.Entity);
			Assert.Equal(id, ii.Id);
			if (ReturnToken)
			{
				var uid = await svc.ParseAccessToken(accessToken);
				Assert.Equal(id, uid);
			}
			else
			{
				var uii = await svc.GetCurIdentity();
				Assert.Equal(id, uii.Id);
				Assert.Equal(name, uii.Name);
				Assert.Equal(icon, uii.Icon);
				Assert.Equal(entity, uii.Entity);
				Assert.Equal(id, uii.Id);
			}

			var uid2 = await sp.IdentitySignin(account, password);
			Assert.Equal(id, uid2);
			return (ii,account,password);
		}
	}
	

}
