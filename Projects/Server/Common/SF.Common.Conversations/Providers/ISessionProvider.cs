
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
	public interface ISessionMemberDesc
	{
		string Name { get; }
		string Icon { get; }
	}

	public interface ISessionProvider 
	{
		string IdentType { get; }
		Task MemberRelationValidate(long BizIdent, long UserId);
		Task<Dictionary<long, ISessionMemberDesc>> GetMemberDesc(long BizIdent, long[] Users);
		
	}
}