using Xunit;
using System.Threading.Tasks;
using SF.UT.Utils;
using System;
using SF.Users.Promotions.MemberInvitations;
using SF.Core.ServiceManagement;
using SF.Entities;
using System.Linq;
using SF.Users.Promotions.MemberInvitations.Models;
using SF.ADT;

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
			var id = 24;
			await Scope(async (IServiceProvider sp) =>
				await InvitationTest(sp,
					async (svc) =>
					{
						var dst = new MemberInvitationInternal
						{
							Id=id,
							Time = DateTime.Now,
							UserId = 20,
							InvitorId = 20,
							Invitors = "[20]"
						};

						var re = await svc.CreateAsync(dst);
						Assert.Equal(dst, re);

						var o = await svc.GetAsync(dst);
						Assert.True(Poco.DeepEquals(dst, o));

						var oss = await svc.GetAsync(new [] { dst});
						Assert.Equal(1, oss.Length);
						Assert.True(Poco.DeepEquals(dst, oss[0]));

						var ids = await svc.QueryIdentsAsync(new MemberInvitationQueryArgument(), Paging.All);
						Assert.Equal(dst.Id, ids.Items.Single().Id);


						var os = await svc.QueryAsync(new MemberInvitationQueryArgument(), Paging.All);
						o = os.Items.Single();
						Assert.True(Poco.DeepEquals(dst, o));


						var e = await svc.LoadForEdit(dst);
						Assert.True(Poco.DeepEquals(dst, e));

						e.Invitors += "1";
						e.InvitorId += 1;
						e.UserId += 1;
						var newTime = dst.Time.AddDays(1);
						e.Time = newTime;
						await svc.UpdateAsync(e);

						o = await svc.GetAsync(dst);
						Assert.True(Poco.DeepEquals(o, e));


						await svc.RemoveAsync(dst);
						Assert.Null(await svc.GetAsync(dst));

						return re;

					}
				)
			);
		}
		
	}


}
