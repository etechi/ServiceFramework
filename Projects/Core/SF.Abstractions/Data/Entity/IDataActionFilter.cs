using SF.Core.ServiceManagement;
using System;
using System.Threading.Tasks;
namespace SF.Data.Entity
{
	[UnmanagedService]

	public interface IDataActionFilterContext : IDisposable
    {
        Task OnModelLoaded();
        Task Completed(Exception Exception);
    }
	[UnmanagedService]

	public interface IDataActionFilter
    {
        Task Evaluate(IDataActionInvocation Invocation);
    }
}