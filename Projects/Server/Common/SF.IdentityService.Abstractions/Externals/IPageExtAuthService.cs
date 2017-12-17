using System.Threading.Tasks;
using System.Net.Http;
using SF.Sys.NetworkService;

namespace SF.Auth.IdentityServices.Externals
{
	[NetworkService]
	public interface IPageExtAuthService
	{
       Task<HttpResponseMessage> Start(
            string Provider, 
            string Callback
            );
		Task<HttpResponseMessage> Callback(string Provider);
	}


}
