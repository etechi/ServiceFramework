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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.CallPlans
{
	/// <summary>
	/// 执行时抛出此异常将引发重复调用，可用于实现重复定时器
	/// </summary>
	public class RepeatCallException : Exception
    {
        public DateTime Target { get; }
		public string NewCallArgument { get; set; }

		public RepeatCallException(DateTime Target)
        {
            this.Target = Target;
        }
    }
	public interface ICallContext
	{
		string Argument { get; }
		string Context { get; }
		Exception Exception { get; }
		object CallData { get; }
	}

    public interface ICallable
	{
		Task Execute(ICallContext CallContext);
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
		/// <param name="CallableName">被调过程名称</param>
		/// <param name="CallContext">调用上下文</param>
		/// <param name="CallArgument">调用参数</param>
		/// <param name="Exception">调用时异常</param>
		/// <param name="Title">调用标题</param>
		/// <param name="CallTime">调用时间，如果为DateTime.Min时，立即调用</param>
		/// <param name="ExpireSeconds">出错时重复尝试的最长时间，超过此时间后，调用被放弃，单位：秒</param>
		/// <param name="DelaySecondsOnError">出错时延时时间，单位：秒</param>
		/// <param name="SkipExecute"></param>
		/// <param name="CallData"></param>
		/// <returns></returns>
		Task<bool> Schedule(
			string CallableName,
			string CallContext,
			string CallArgument,
			Exception Exception,
			string Title,
			DateTime CallTime,
			int ExpireSeconds,
			int DelaySecondsOnError,
            bool SkipExecute = false,
			object CallData=null
            );
        /// <summary>
        /// 取消调用
        /// </summary>
        /// <remarks>CallableName+CallContext必须全局唯一</remarks>
        /// <param name="CallableName">被调过程名称</param>
        /// <param name="CallContext">调用上下文</param>
        Task Cancel(
            string CallableName,
            string CallContext
            );

		/// <summary>
		/// 立即执行
		/// </summary>
		/// <remarks>CallableName+CallContext必须全局唯一</remarks>
		/// <param name="CallableName">被调过程名称</param>
		/// <param name="CallContext">调用上下文</param>
		/// <param name="CallData"></param>
		Task Execute(
            string CallableName,
            string CallContext,
			object CallData
            );
    }
	public interface ICallDispatcher
	{
		Task<int> SystemStartupCleanup();
		Task<Task[]> Execute(int count);
		Task Execute(string ident,object ExecData);
	}
}
