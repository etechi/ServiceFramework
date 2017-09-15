
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Clients
{

    public interface IClientService
	{
		IAccessSource AccessSource { get; }

		long? CurrentScopeId { get; }
		string GetAccessToken();
		Task SetAccessToken(string AccessToken);

		//Task<string> Ensure(AccessInfo ClientInfo,Dictionary<string,string> Items);

		//Task SetItems(string Session,Dictionary<string, string> Values);
		//Task<Dictionary<string, string>> GetItems(string Session);
		//Task ClearItems(string Session,string[] Options);
	}
}
