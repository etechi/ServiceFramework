using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data.Storage;
using Xunit;
using SF.Applications;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.ServiceFeatures;
using System.Threading.Tasks;
using SF.Auth.Identities;
using SF.Data;

namespace SF.UT.Auth
{
	public class IdentityTest : TestBase
	{
		[Fact]
		public async Task Create()
		{
			var ig = ServiceProvider.Resolve<IIdentGenerator>();

			await ServiceProvider.TestService(
				sim => sim.NewAuthIdentityServive(),
				async svc =>
				{
					var account = "user"+await ig.GenerateAsync("测试ID");
					var password = "123456";
					var id = await ig.GenerateAsync("测试用户");

					//var VerifyCode = await svc.SendCreateIdentityVerifyCode(
					//	new SendCreateIdentityVerifyCodeArgument
					//	{
					//		Credetial = phone
					//	});

					var accessToken = await svc.CreateIdentity(
						new CreateIdentityArgument
						{
							Credential = account,
							Password = password,
							Identity = new SF.Auth.Identities.Models.Identity
							{
								Id = id,
								Entity = "测试用户",
								Name = "测试用户1"
							},
							ReturnToken=true
						}, true
						);

					var uid = await svc.ParseAccessToken(accessToken);
					Assert.Equal(id, uid);

					var signinResult = await svc.Signin(new SigninArgument
					{
						Credential = account,
						Password = password,
						ReturnToken=true
					});
					var uid2 = await svc.ParseAccessToken(signinResult);
					Assert.Equal(id, uid2);
				},
				async (sp,sim,svc)=>
				{
					await sim.NewDataProtectorService().Ensure(sp, svc);
					await sim.NewPasswordHasherService().Ensure(sp, svc);
				});

		}
	}
	

}
