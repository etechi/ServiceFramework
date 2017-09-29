using SF.Metadata;
using SF.Auth;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Entities;
using SF.Data;
using SF.Core.ServiceManagement;
using SF.Core;

namespace SF.Users.Members
{
	public static class MemberExtension
	{
		public static async Task<MemberInternal> MemberEnsure(
			this IMemberService Service,
			IServiceProvider ServiceProvider,
			string name,
			string nick,
			string phoneNumber,
			string password,
			string[] roles = null,
			Dictionary<string, string> extArgs = null
			)
		{
			var sid = ((IManagedServiceWithId)Service).ServiceInstanceId;
			var ims = ServiceProvider.Resolve<IMemberManagementService>(null, sid);

			if (roles != null && roles.Length > 0)
			{
				if (extArgs == null)
					extArgs = new Dictionary<string, string>();
				extArgs["roles"] = Json.Stringify(roles);
			}

			var sess = await Service.Signup(
				new CreateMemberArgument
				{
					Credential = phoneNumber,					
					ExtraArgument = extArgs.Count > 0 ? extArgs : null,
					Identity = new Identity
					{
						Icon = null,
						Name = nick
					},
					Password = password,
					ReturnToken = true,
				}
				);

			var identityService = ServiceProvider.Resolve<IIdentityService>();
			var id=await identityService.ParseAccessToken(sess);
			return await ims.GetAsync(Entity<MemberInternal>.WithKey(id));

		}
	}

}

