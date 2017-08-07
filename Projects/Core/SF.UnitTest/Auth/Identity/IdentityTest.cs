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
			var ids = ServiceProvider.Resolve<IIdentityService>();
			var phone = "13112341234";
			var id = await ig.GenerateAsync("测试用户");
			var VerifyCode = await ids.SendCreateIdentityVerifyCode(new SendCreateIdentityVerifyCodeArgument
			{
				Credetial = phone
			});

			var accessToken = await ids.CreateIdentity(
				new CreateIdentityArgument
				{
					Credential = phone,
					Password = "11111111",
					Identity = new SF.Auth.Identities.Models.Identity
					{
						Id = id,
						Entity = "测试用户",
						Name = "测试用户1"
					},
					VerifyCode = VerifyCode
				}, true
				);

			var uid=await ids.ParseAccessToken(accessToken);
			Assert.Equal(id, uid);

		}
	}
	

}
