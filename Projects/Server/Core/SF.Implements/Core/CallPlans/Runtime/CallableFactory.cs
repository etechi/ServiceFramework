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
using SF.Core.Logging;
using SF.Core.ServiceManagement;
using System.Threading;

namespace SF.Core.CallPlans.Runtime
{
	public class CallableFactory
	{
		Dictionary<string, Func<IServiceProvider, long?, ICallable>> Creators { get; }
		public CallableFactory(IEnumerable<ICallableDefination> callables)
		{
			Creators = callables.ToDictionary(c => c.Type, c => c.CallableCreator);
		}
		public bool Exists(string Name)
		{
			return Creators.ContainsKey(Name);
		}
		public ICallable Create(IServiceProvider ServiceProvider, string Name,long? Id)
		{
			var f = Creators.Get(Name);
			if (f == null)
				throw new ArgumentException("找不到调用接口:" + Name);
			return f(ServiceProvider, Id);
		}
	}
}
