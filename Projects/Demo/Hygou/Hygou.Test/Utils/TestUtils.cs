#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using Xunit;
using SF.Core.ServiceManagement;
using System.Threading.Tasks;
using SF.Data;
using SF.Auth.Users;
using SF.Auth.Users.Models;

namespace SF.UT.Utils
{
	public static class IdentityTestUtils
	{
		public static async Task<T> IdentityTest<T>(this IServiceProvider sp,Func<IUserService, Task<T>> Action)
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
			var svc = sp.Resolve<IUserService>();
			var signinResult = await svc.Signin(new SigninArgument
			{
				Credential = account,
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
			var svc = sp.Resolve<IUserService>();

			id = id == 0 ? await ig.GenerateAsync("测试用户") : id;
			account = account ?? "user" + id;
			name = name ?? "测试用户" + id;
			var icon = "icon" + id;
			var accessToken = await svc.Signup(
				new SignupArgument
				{
					Credential = account,
					Password = password,
					User = new SF.Auth.Users.Models.Identity
					{
						Id = id,
						OwnerId = entity,
						Name = name,
						Icon= icon
					},
					ReturnToken = ReturnToken
				}, true
				);

			var uid = ReturnToken ? await svc.ValidateAccessToken(accessToken) : (await svc.GetCurUser()).Id;
			var ii = await svc.GetIdentity(uid);
			Assert.Equal(name, ii.Name);
			Assert.Equal(icon, ii.Icon);
			Assert.Equal(entity, ii.OwnerId);
			Assert.Equal(uid, ii.Id);
			if (!ReturnToken)
			{
				var uii = await svc.GetCurUser();
				Assert.Equal(uid, uii.Id);
				Assert.Equal(name, uii.Name);
				Assert.Equal(icon, uii.Icon);
				Assert.Equal(entity, uii.OwnerId);
			}

			var uid2 = await sp.IdentitySignin(account, password);
			Assert.Equal(uid, uid2);
			return (ii,account,password);
		}
	}
	

}
