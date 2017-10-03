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

namespace SF.Users.Members
{
	public class MemberService :
		IMemberService,
		IManagedServiceWithId
	{
		MemberServiceSetting Setting { get; }
		IServiceInstanceDescriptor ServiceInstanceDescriptor { get; }
		public long ServiceInstanceId => ServiceInstanceDescriptor.InstanceId;

		public MemberService(MemberServiceSetting Setting, IServiceInstanceDescriptor ServiceInstanceDescriptor) 
		{
			this.Setting = Setting;
			this.ServiceInstanceDescriptor = ServiceInstanceDescriptor;
		}

		[TransactionScope("用户注册")]
		public async Task<string> Signup(CreateMemberArgument Arg)
		{
			var token = await Setting.ManagementService.Value.CreateMemberAsync(
				Arg
				);
			return token;
		}

		public Task<MemberDesc> GetCurrentMember()
		{
			return null;
		}
	}

}

