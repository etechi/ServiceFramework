#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Plans
{
	public interface ICallContext
	{
		long? ServiceScopeId { get;  }
		string Type { get; }
		string Ident { get; }
		string Argument { get; }
		Exception Exception { get; }
		object CallData { get; }
	}
	
	public interface ICallResult
	{
	}
	
	public class RepeatCallResult : ICallResult
	{
		public DateTime NextCallTime { get; set; }
		public string NextCallArgument { get; set; }
	}
    public interface ICallable
	{
		Task<ICallResult> Execute(ICallContext CallContext);
	}
	public interface ICallableDefination
	{
		string Type { get; }
		Func<IServiceProvider,long?,ICallable> CallableCreator { get; }
	}
	public interface ICallPlanProvider
	{
		/// <summary>
		/// 创建一个调用计划
		/// </summary>
		/// <remarks>CallableName+CallContext必须全局唯一</remarks>
		/// <param name="Type">被调过程类型</param>
		/// <param name="Ident">调用标识</param>
		/// <param name="Argument">调用参数</param>
		/// <param name="Exception">调用时异常</param>
		/// <param name="Title">调用标题</param>
		/// <param name="CallTime">调用时间，如果为DateTime.Min时，立即调用</param>
		/// <param name="ExpireSeconds">出错时重复尝试的最长时间，超过此时间后，调用被放弃，单位：秒</param>
		/// <param name="DelaySecondsOnError">出错时延时时间，单位：秒</param>
		/// <param name="SkipExecute"></param>
		/// <param name="CallData"></param>
		/// <param name="ServiceScopeId"></param>
		/// <returns></returns>
		Task<bool> Schedule(
			string Type,
			string Ident,
			string Argument,
			Exception Exception,
			string Title,
			DateTime CallTime,
			int ExpireSeconds,
			int DelaySecondsOnError,
            bool SkipExecute = false,
			object CallData=null,
			long? ServiceScopeId=null
            );
		/// <summary>
		/// 取消调用
		/// </summary>
		/// <remarks>CallableName+CallContext必须全局唯一</remarks>
		/// <param name="Type">被调过程类型</param>
		/// <param name="Ident">调用标识</param>
		Task Cancel(
            string Type,
            string Ident
            );

		/// <summary>
		/// 立即执行
		/// </summary>
		/// <remarks>CallableName+CallContext必须全局唯一</remarks>
		/// <param name="Type">被调过程类型</param>
		/// <param name="Ident">调用标识</param>
		/// <param name="CallData"></param>
		Task Execute(
            string Type,
            string Ident,
			object CallData
            );
    }
	public interface ICallDispatcher
	{
		Task<int> SystemStartupCleanup();
		Task<Task[]> Execute(int count);
		Task Execute(string Type,string Ident,object ExecData);
	}

	public interface IProcess:IEntityWithId<long>
	{
		DateTime CallTime { get; }
		
	}
	public class ProcessResult
	{
		public DateTime? NextCallTime { get; set; }
		public string Error { get; set; }
	}
	
	public interface IProcessStorage<TProcessInstance>
	{

	}
	public interface IProcessGuarantee
	{
		IDisposable Create<TKey, TProcess>(
			Func<Task> Clieanup,
			Func<int, DateTime, Task<TKey[]>> LoadIdents,
			Func<TKey, Task<TProcess>> LoadProcess,
			Func<TKey, ProcessResult, Task> SaveProcess,
			Func<TProcess,Task<ProcessResult>> Process
			)
			where TProcess : class, IProcess;
	}
}
