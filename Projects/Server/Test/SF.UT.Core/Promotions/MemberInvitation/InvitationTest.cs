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
			var id = 21;
			await Scope(async (IServiceProvider sp) =>
				await InvitationTest(sp,
					async (svc) =>
					{
						var time = DateTime.Now;
						var UserId = 20;
						var InvitorId = 20;
						var Invitors = "[20]";

						var re = await svc.CreateAsync(
							new Users.Promotions.MemberInvitations.Models.MemberInvitationInternal
						{
							Id = id,
							Time = time,
							UserId = UserId,
							InvitorId= InvitorId,
							Invitors= Invitors
							});
						Assert.Equal(id, re);

						var e = await svc.GetAsync(id);
						Assert.NotNull(e);
						Assert.Equal(Invitors, e.Invitors);
						Assert.Equal(InvitorId, e.InvitorId);
						Assert.Equal(UserId, e.UserId);
						Assert.Equal(id, e.Id);

						return re;

					}
				)
			);
		}
		
	}


}
