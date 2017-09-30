using SF.Auth.Sessions.Models;
using SF.Entities;
using SF.Metadata;
using System;
using System.Threading.Tasks;

namespace SF.Auth.Sessions
{
	public class UserSessionQueryArgument : Entities.IQueryArgument<ObjectKey<long>>
	{
		public ObjectKey<long> Id { get; set; }
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
		Entities.IEntitySource<UserSessionInternal,UserSessionQueryArgument>
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

