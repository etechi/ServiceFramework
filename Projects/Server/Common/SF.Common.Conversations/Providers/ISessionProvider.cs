
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using SF.Sys.Annotations;
using SF.Common.Conversations.Models;
using SF.Sys.Auth;
using System;
using SF.Sys.Services;
using System.Linq;
using System.Collections.Generic;

namespace SF.Common.Conversations.Providers
{
	
	public interface ISessionProvider 
	{
		Task MemberRelationValidate(long BizIdent, long UserId);
		Task<Dictionary<long, Front.SessionMember>> GetMemberDesc(long BizIdent, long[] Users);
		Task<Front.SessionMember[]> QueryMembers(long BizIdent);
		Task<Front.SessionGroup[]> QuerySessions(long UserId);
		
	}
}