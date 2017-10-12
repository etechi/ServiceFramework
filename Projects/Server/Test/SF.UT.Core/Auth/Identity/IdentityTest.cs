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

using Xunit;
using System.Threading.Tasks;
using SF.UT.Utils;
using System;

namespace SF.UT.Auth
{
	public class IdentityTest : TestBase
	{

		[Fact]
		public async Task 创建_成功_令牌()
		{
			await Scope(async (IServiceProvider sp) =>
				await sp.IdentityTest(
					async (svc) =>
						await sp.IdentityCreate(ReturnToken: true)
				)
			);
		}
		[Fact]
		public async Task 创建_成功_会话()
		{
			await Scope(async (IServiceProvider sp) =>
				await sp.IdentityTest(
					async ( svc) => 
						await sp.IdentityCreate(ReturnToken:false)
					)
			);
		}
		[Fact]
		public async Task 创建_账号重复()
		{
			await Scope(async (IServiceProvider sp) =>
				await sp.IdentityTest(
					async (svc) =>
					{ 
						var re =await sp.IdentityCreate();
						await Assert.ThrowsAsync<PublicArgumentException>(async () =>
						{
							await sp.IdentityCreate(0,re.account);
						});
						return 0;
					}
				)
			);
		}
		[Fact]
		public async Task 登录_成功_令牌()
		{
			await Scope(async (IServiceProvider sp) =>
				await sp.IdentityTest(
					async (svc) =>
					{
						var re = await sp.IdentityCreate();
						var uid2 = await sp.IdentitySignin(re.account, re.password, returnToken: true);
						Assert.Equal(re.identity.Id, uid2);
						return 0;
					})
			);
		}
		[Fact]
		public async Task 登录_成功_会话()
		{

			await Scope(async (IServiceProvider sp) =>
				await sp.IdentityTest(
					async ( svc) =>
					{
						var re = await sp.IdentityCreate();
						var uid2 = await sp.IdentitySignin(re.account, re.password, returnToken: false);
						Assert.Equal(re.identity.Id, uid2);
						return 0;
					})
			);
		}
		[Fact]
		public async Task 登录_密码错误()
		{
			await Scope(async (IServiceProvider sp) =>
				await sp.IdentityTest(
					async (svc) =>
					{
						var re = await sp.IdentityCreate();
						await Assert.ThrowsAsync<PublicArgumentException>(async () =>
						{
							await sp.IdentitySignin(re.account, re.password + "123");
						});
						return 0;
					})
			);
		}
		[Fact]
		public async Task 修改密码()
		{
			await Scope(async (IServiceProvider osp) =>
				  await osp.IdentityTest(
					  async ( svc) =>
					  {
						  var acc = await Scope(async (IServiceProvider sp) =>
						  {
							  var re = await sp.IdentityCreate(ReturnToken: false);
							  var newPassword = re.password + "123";
							  await svc.SetPassword(
									  new SF.Auth.Identities.SetPasswordArgument
									  {
										  NewPassword = newPassword,
										  OldPassword = re.password
									  });
							  await svc.Signout();
							  Assert.False((await svc.GetCurIdentityId()).HasValue);
							  return (identity: re.identity, account: re.account, password: re.password, newPassword: newPassword);
						  });
						  await Scope(async (IServiceProvider sp) =>
						  {
							  await Assert.ThrowsAsync<PublicArgumentException>(async () =>
								{
									await sp.IdentitySignin(acc.account, acc.password);
								});
							  var re = await sp.IdentitySignin(acc.account, acc.newPassword);
							  Assert.Equal(acc.identity.Id, re);
							  return 0;
						  });
						  return 0;
					  })
			);
		}

		//[Fact]
		//public async Task 忘记密码()
		//{
		//	await ServiceProvider.IdentityTest(async (sp, svc) =>
		//	{
		//		var re = await sp.IdentityCreate();
		//		Assert.Equal(re.identity.Id, uid2);
		//		return 0;
		//	});
		//}
	}


}
