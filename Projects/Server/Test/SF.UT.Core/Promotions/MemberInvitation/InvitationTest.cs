using Xunit;
using System.Threading.Tasks;
using SF.UT.Utils;
using System;
using SF.Users.Promotions.MemberInvitations;
using SF.Core.ServiceManagement;
namespace SF.UT.Promotions.Invitations
{
	public class InvitationTests : TestBase
	{
		public static async Task<T> InvitationTest<T>(IServiceProvider sp, Func<IMemberInvitationManagementService, Task<T>> Action)
		{
			return await sp.TestService(
				sim => sim.NewMemberInvitationServive(),
				Action
				);
		}

		[Fact]
		public async Task 创建_成功()
		{
			await Scope(async (IServiceProvider sp) =>
				await InvitationTest(sp,
					async (svc) =>
					{
						var re = await svc.CreateAsync(
							new Users.Promotions.MemberInvitations.Models.MemberInvitationInternal
						{
							Id = 12,
							Time = DateTime.Now,
							UserId = 20,
							InvitorId=20,
							Invitors= $"[{20}]"
							});
						Assert.Equal(12, re);
						return re;

					}
				)
			);
		}
		
	}


}
