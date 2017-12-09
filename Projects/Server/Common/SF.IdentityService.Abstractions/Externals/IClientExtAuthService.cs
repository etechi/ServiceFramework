using System.Collections.Generic;
using System.Threading.Tasks;
using SF.Sys.Auth;
using SF.Sys.NetworkService;

namespace SF.Auth.IdentityServices.Externals
{
	public class ExtAuthArgument
	{
		public string Provider { get; set; }
		public string State { get; set; }
		public Dictionary<string, string> Arguments { get; set; }
	}
	public class AuthCallbackArgument
	{
		public string State { get; set; }
		public Dictionary<string, string> Arguments { get; set; }
	}
	public class AuthCallbackResult
	{
		public string AccessToken { get; set; }
		public User User { get; set; }
	}
	public interface IClientExtAuthService
	{
		[HeavyMethod]
		Task<ExtAuthArgument> GetAuthArgument(string Provider,string ClientId);

		[HeavyMethod]
		Task<AuthCallbackResult> AuthCallback(AuthCallbackArgument Arg);
	}


}
