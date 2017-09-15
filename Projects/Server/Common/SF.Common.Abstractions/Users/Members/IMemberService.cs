using SF.Metadata;
using SF.Auth;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Data.Models;
using SF.Core.Events;

namespace SF.Users.Members
{
	public class MemberRegisted  : IEvent
	{
		public long ServiceId { get; set; }
		public long MemberId { get; set; }
		public DateTime Time { get; set; }

		long? IEvent.IdentityId => MemberId;
		long? IEvent.ServiceId => ServiceId;
	}
	
	[NetworkService]
	public interface IMemberService
	{
		Task<string> Signup(CreateMemberArgument Arg);
		Task<MemberDesc> GetCurrentMember();
	}

}

