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

namespace SF.Core.CallPlans
{

	public static class ICallPlanProviderExtension
	{
		public static Task DelayCall(
			this ICallPlanProvider Provider,
			string CallableName,
			string CallContext,
			string CallArgument,
			Exception Exception,
			string Title,
			DateTime CallTime,
			int ExpireSeconds=365*86400,
			int DelaySecondsOnError=5*60
			)
		{
			return Provider.Schedule(
				CallableName,
				CallContext,
				CallArgument,
				Exception,
				Title,
				CallTime,
				ExpireSeconds,
				DelaySecondsOnError
				);
		}

		public static Task Call(
			this ICallPlanProvider Provider,
			string CallableName,
			string CallContext,
			string Argument,
			Exception Exception,
			string Title,
			int ExpireSeconds,
			int DelaySecondsOnError
			)
		{
			return Provider.Schedule(
				CallableName,
				CallContext,
				Argument,
				Exception,
				Title,
				DateTime.MinValue,
				ExpireSeconds,
				DelaySecondsOnError
				);
		}
	}
}
