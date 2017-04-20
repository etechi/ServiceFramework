using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Auth.Sessions.Models;
using SF.Metadata;

namespace SF.Auth.Sessions
{
	public class UserSessionQueryArgument : Data.Entity.IQueryArgument<long>
	{
		public Option<long> Id { get; set; }
		public string NickName { get; set; }
	}

	public class SessionConfig
	{
		public long IdentId { get; set; }
		public DateTime? Expires { get; set; }
	}
	public delegate Task<string> SessionCreator(long UserId);
	[NetworkService]
	public interface ISessionService : 
		Data.Entity.IEntitySource<long,UserSessionInternal,UserSessionQueryArgument>
	{
		Task<T> Create<T>(
			int ScopeId,
			string IdentValue,
			string IdentProvider,
			Clients.IAccessSource AccessInfo, 
			Func<SessionCreator, Task<T>> Callback
			);
		//Task<long> Create(long UserId, DateTime? Expires, Clients.IAccessSource AccessInfo);
		Task Update(long Id);
		Task Signout(long Id);
	}

}

