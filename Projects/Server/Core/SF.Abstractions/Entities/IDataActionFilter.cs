using SF.Core.ServiceManagement;
using System;
using System.Threading.Tasks;
namespace SF.Entities
{

	public interface IDataActionFilterContext : IDisposable
    {
        Task OnModelLoaded();
        Task Completed(Exception Exception);
    }

	public interface IDataActionFilter
    {
        Task Evaluate(IDataActionInvocation Invocation);
    }
}