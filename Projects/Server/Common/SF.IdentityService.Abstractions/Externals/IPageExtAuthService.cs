using System.Threading.Tasks;
using System.Net.Http;

namespace SF.Auth.IdentityServices.Externals
{
	public interface IPageExtAuthService
	{
       Task<HttpResponseMessage> Start(
            string Provider, 
            string Callback
            );
		Task<HttpResponseMessage> Callback(string Provider);
	}


}
