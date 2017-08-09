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
					var account = "user1";
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
							Password = "11111111",
							Identity = new SF.Auth.Identities.Models.Identity
							{
								Id = id,
								Entity = "测试用户",
								Name = "测试用户1"
							}
						}, true
						);

					var uid = await svc.ParseAccessToken(accessToken);
					Assert.Equal(id, uid);
				});

		}
	}
	

}
