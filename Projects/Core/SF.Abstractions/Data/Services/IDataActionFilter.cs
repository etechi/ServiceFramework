using System;
using System.Threading.Tasks;
namespace SF.Data.Services
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