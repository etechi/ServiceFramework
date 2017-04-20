using SF.Auth.Identity.Models;
using SF.Core.ManagedServices.Models;
using SF.KB;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;

namespace SF.Auth.Identity
{
	
	public interface IIdentAdminService
    {
		Task<ServiceInstance[]> GetBindProviders();
		IIdentBindProvider GetBindProvider(string ProviderId);
	}

}

