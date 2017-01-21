using ServiceProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sessions
{
	public class SessionCreateArgument
	{
		public ClientAccessInfo ClientInfo { get; set; }
		public Dictionary<string,string> Options { get; set; }
	}
	public interface ISessionFactory
	{
		Task<string> Create(SessionCreateArgument Argument);
	}
	public interface ISessionService {
		string Id { get; }

		Task Cleanup();

		Task SetOptions(Dictionary<string, string> Values);
		Task<Dictionary<string, string>> GetOptions();
		Task ClearOptions(string[] Options);

		string UserId { get; }

		Task BindUser(string UserId);
		Task UnbindUser();
	}
}
