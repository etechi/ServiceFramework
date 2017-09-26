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
