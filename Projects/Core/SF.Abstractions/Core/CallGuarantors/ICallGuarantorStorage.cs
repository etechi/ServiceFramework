using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.CallGuarantors
{
	public interface ICallInstance
	{
		string Callable { get; }
		string CallArgument { get; }
		string CallError { get; }
		DateTime Expire { get; }
		DateTime CallTime { get; }
		int DelaySecondsOnError { get; }
	}
	
	public interface ICallStorageAction
	{
		
	}
	public interface ICallGuarantorStorage
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Callable"></param>
        /// <param name="CallArgument"></param>
        /// <param name="Error"></param>
        /// <param name="Title"></param>
        /// <param name="Now"></param>
        /// <param name="CallTime"></param>
        /// <param name="ExpireTime"></param>
        /// <param name="DelaySecondsOnError"></param>
        /// <returns>调用已存在时返回false,否则为true</returns>
		Task<bool> Create(
			string Callable,
			string CallArgument,
			string Error,
			string Title,
			DateTime Now,
			DateTime CallTime,
			DateTime ExpireTime,
			int DelaySecondsOnError
			);
        Task Remove(string Callable);

        Task<string[]> GetOnTimeInstances(
			int Count,
			DateTime now, 
			DateTime ExecutingStartTime, 
			DateTime InitTime
			);

		Task<ICallInstance> GetInstance(string Id);
		Task<ICallInstance[]> GetInstancesForCleanup(DateTime ExecutingStartTime);
		ICallStorageAction CreateExpiredAction(
			ICallInstance Instance,
			DateTime Now,
			string Error
			);
		ICallStorageAction CreateRetryAction(
			ICallInstance Instance,
			DateTime NewCallTime,
			bool Expired,
			string Error
			);
		ICallStorageAction CreateSuccessAction(ICallInstance Instance);
		Task ExecuteActions(IEnumerable<ICallStorageAction> Actions);
	}
}
