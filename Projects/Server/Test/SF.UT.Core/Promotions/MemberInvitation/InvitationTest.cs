using Xunit;
using System.Threading.Tasks;
using SF.UT.Utils;
using System;
using SF.Users.Promotions.MemberInvitations;
using SF.Core.ServiceManagement;
using SF.Entities;
using System.Linq;

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

						var o = await svc.GetAsync(id);
						Assert.NotNull(o);
						Assert.Equal(Invitors, o.Invitors);
						Assert.Equal(InvitorId, o.InvitorId);
						Assert.Equal(UserId, o.UserId);
						Assert.Equal(time, o.Time);
						Assert.Equal(id, o.Id);

						var oss = await svc.GetAsync(new long[] { id });
						Assert.Equal(1, oss.Length);
						o = oss[0];
						Assert.NotNull(o);
						Assert.Equal(Invitors, o.Invitors);
						Assert.Equal(InvitorId, o.InvitorId);
						Assert.Equal(UserId, o.UserId);
						Assert.Equal(time, o.Time);
						Assert.Equal(id, o.Id);

						var ids = await svc.QueryIdentsAsync(new MemberInvitationQueryArgument(), Paging.All);
						Assert.Equal(id, ids.Items.Single());


						var os = await svc.QueryAsync(new MemberInvitationQueryArgument(), Paging.All);
						o = os.Items.Single();
						Assert.NotNull(o);
						Assert.Equal(Invitors, o.Invitors);
						Assert.Equal(InvitorId, o.InvitorId);
						Assert.Equal(UserId, o.UserId);
						Assert.Equal(time, o.Time);
						Assert.Equal(id, o.Id);


						var e = await svc.LoadForEdit(id);
						Assert.NotNull(e);
						Assert.Equal(Invitors, e.Invitors);
						Assert.Equal(InvitorId, e.InvitorId);
						Assert.Equal(UserId, e.UserId);
						Assert.Equal(time, e.Time);
						Assert.Equal(id, e.Id);

						e.Invitors += "1";
						e.InvitorId += 1;
						e.UserId += 1;
						var newTime = time.AddDays(1);
						e.Time = newTime;
						await svc.UpdateAsync(e);

						o = await svc.GetAsync(id);
						Assert.NotNull(o);
						Assert.Equal(Invitors+"1", o.Invitors);
						Assert.Equal(InvitorId+1, o.InvitorId);
						Assert.Equal(UserId+1, o.UserId);
						Assert.Equal(newTime, o.Time);
						Assert.Equal(id, o.Id);


						await svc.RemoveAsync(id);
						Assert.Null(await svc.GetAsync(id));

						return re;

					}
				)
			);
		}
		
	}


}
