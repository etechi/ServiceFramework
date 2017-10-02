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
using SF.Core.NetworkService;

namespace SF.UT.Core.NetworkServices
{
	public class MetadataTest : TestBase
	{
		[Fact]
		public async Task JsonTest()
		{
			await Scope((IServiceProvider sp) =>
			{
				var sms = sp.Resolve<IServiceMetadataService>();
				var lib=sms.Json();
				Assert.NotNull(lib);
				return Task.FromResult(1);
			});
		}
		
	}


}
