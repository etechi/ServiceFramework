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

namespace SF.Sys.Plans.Manager
{
	public interface ICallInstance
	{
		string Type { get; }
		string Ident { get; }
		string Argument { get; }
		string Error { get; }
		DateTime Expire { get; }
		DateTime CallTime { get; }
		int DelaySecondsOnError { get; }
		long? ServiceScopeId { get; }
	}
	
	public interface ICallPlanStorageAction
	{
		
	}
	public interface ICallPlanStorage
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Type"></param>
		/// <param name="Ident"></param>
		/// <param name="Argument"></param>
		/// <param name="Error"></param>
		/// <param name="Title"></param>
		/// <param name="Now"></param>
		/// <param name="CallTime"></param>
		/// <param name="ExpireTime"></param>
		/// <param name="DelaySecondsOnError"></param>
		/// <param name="ServiceScopeId"></param>
		/// <returns>调用已存在时返回false,否则为true</returns>
		Task<bool> Create(
			string Type,
			string Ident,
			string Argument,
			string Error,
			string Title,
			DateTime Now,
			DateTime CallTime,
			DateTime ExpireTime,
			int DelaySecondsOnError,
			long? ServiceScopeId
			);
        Task Remove(string Type,string Ident);

        Task<(string Type,string Ident)[]> GetOnTimeInstances(
			int Count,
			DateTime now, 
			DateTime ExecutingStartTime, 
			DateTime InitTime
			);

		Task<ICallInstance> GetInstance(string Type,string Ident);
		Task<ICallInstance[]> GetInstancesForCleanup(DateTime ExecutingStartTime);
		ICallPlanStorageAction CreateExpiredAction(
			ICallInstance Instance,
			DateTime Now,
			string Error
			);
		ICallPlanStorageAction CreateRetryAction(
			ICallInstance Instance,
			DateTime NewCallTime,
			bool Expired,
			string Error,
			string newCallArgument
			);
		ICallPlanStorageAction CreateSuccessAction(ICallInstance Instance);
		Task ExecuteActions(IEnumerable<ICallPlanStorageAction> Actions);
	}
}
