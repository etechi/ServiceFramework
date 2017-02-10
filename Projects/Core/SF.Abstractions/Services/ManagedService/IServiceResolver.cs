using System.Threading.Tasks;

namespace SF.Services.ManagedServices
{
	public interface INamedServiceResolver
	{
		Task<Models.ServiceInstance> GetInstance(string Id);
		object Resolve(string Id);
	}
}
