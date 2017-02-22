﻿
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Clients
{

    public interface IClientService
    {
		Task<string> Ensure(AccessInfo ClientInfo,Dictionary<string,string> Items);
		Task SetItems(string Session,Dictionary<string, string> Values);
		Task<Dictionary<string, string>> GetItems(string Session);
		Task ClearItems(string Session,string[] Options);
	}
}