
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Clients
{

    public interface IClientService
	{
		IAccessSource AccessSource { get; }

		ISession GetSession(int ScopeId);
		Task BindSession(int ScopeId,ISession Session,DateTime? Expires);
		Task ClearSession(int ScopeId);

		//Task<string> Ensure(AccessInfo ClientInfo,Dictionary<string,string> Items);

		//Task SetItems(string Session,Dictionary<string, string> Values);
		//Task<Dictionary<string, string>> GetItems(string Session);
		//Task ClearItems(string Session,string[] Options);
	}
}
