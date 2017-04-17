using System.Threading.Tasks;

namespace SF.Core.ManagedServices
{
	public interface INamedServiceResolver
	{
		Task<Models.ServiceInstanceInternal> GetInstance(string Id);
		object Resolve(string Id);
	}
}
