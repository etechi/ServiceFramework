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

using System.Threading.Tasks;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.Sys.UnitTest;
using SF.Sys.Hosting;
using SF.Auth.IdentityServices;
using SF.Sys.Services;
using SF.Sys;

namespace SF.IdentityService.UnitTest
{
	
	public abstract class UserTest : TestBase
	{
		public UserTest(IAppInstanceBuilder builder) : base(builder) { }
	
		[TestMethod]
		[Ignore]
		public async Task 创建_成功_令牌()
		{
			await NewServiceScope().Use(async (svc,ct) =>
				await svc.UserCreate(ReturnToken: true)
			);
		}
		[TestMethod]
		public async Task 创建_成功_会话()
		{
			await NewServiceScope().Use(async (svc, ct) =>
				await svc.UserCreate(ReturnToken: false)
			);
		}
		[TestMethod]
		public async Task 创建_账号重复()
		{
			await NewServiceScope().Use(async (svc, ct) =>
			{
				var re = await svc.UserCreate();
				await Assert.ThrowsExceptionAsync<PublicArgumentException>(async () =>
				{
					await svc.UserCreate(null,re.Account);
				});
				return 0;
			}
			);
		}
		[TestMethod]
		[Ignore]
		public async Task 登录_成功_令牌()
		{
			await NewServiceScope().Use(async (svc, ct) =>
			{
				var re = await svc.UserCreate();
				var uid2 = await svc.UserSignin(re.Account, re.Password, returnToken: true);
				Assert.AreEqual(re.User.Id, uid2);
				return 0;
			});
		}
		[TestMethod]
		public async Task 登录_成功_会话()
		{
			await NewServiceScope().Use(async (svc, ct) =>
			{
				var re = await svc.UserCreate();
				var uid2 = await svc.UserSignin(re.Account, re.Password, returnToken: false);
				Assert.AreEqual(re.User.Id, uid2);
				return 0;
			});
		}
		[TestMethod]
		public async Task 登录_密码错误()
		{
			await NewServiceScope().Use(async (svc, ct) =>
			{
				var re = await svc.UserCreate();
					await Assert.ThrowsExceptionAsync<PublicArgumentException>(async () =>
					{
						await svc.UserSignin(re.Account, re.Password + "123");
					});
					return 0;
				});
		}
		[TestMethod]
		public async Task 修改昵称头像()
		{
			await NewServiceScope().Use(async (svc, ct) =>
			{
				var re = await svc.UserCreate();
				await svc.UserUpdateNameAndIcon();
				return 0;
			});
		}
		[TestMethod]
		public async Task 修改密码()
		{
			await NewServiceScope().Use(async (osp, ct) =>
			{
				var acc = await osp
					.AsScope().NewServiceScope()
					.Use(async (sp) =>
					{
						var re = await sp.UserCreate(ReturnToken: false);
						var newPassword = re.Password + "123";
						var svc = sp.Resolve<IUserService>();
						await svc.SetPassword(
								new SetPasswordArgument
								{
									NewPassword = newPassword,
									OldPassword = re.Password,
									ClientId= "app.android"
								});
						await svc.Signout();
						Assert.IsFalse((await svc.GetCurUserId()).HasValue);
						return (user:re.User, account: re.Account, password: re.Password, newPassword: newPassword);
					});
				await osp
					.AsScope().NewServiceScope()
					.Use(async (sp) =>
					{
						await Assert.ThrowsExceptionAsync<PublicArgumentException>(async () =>
						{
							await sp.UserSignin(acc.account, acc.password);
						});
						var re = await sp.UserSignin(acc.account, acc.newPassword);
						Assert.AreEqual(acc.user.Id, re);
						return 0;
					});
				return 0;
			});
		}

	
	}


}
