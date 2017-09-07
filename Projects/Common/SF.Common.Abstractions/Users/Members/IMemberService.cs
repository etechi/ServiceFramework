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

namespace SF.Users.Members
{
	[Event]
	public class MemberRegisted 
	{
		public long ServiceId { get; set; }
		public long MemberId { get; set; }
	}
	
	[NetworkService]
	public interface IMemberService
	{
		Task<string> Signup(CreateMemberArgument Arg);
		Task<MemberDesc> GetCurrentMember();
	}

}

