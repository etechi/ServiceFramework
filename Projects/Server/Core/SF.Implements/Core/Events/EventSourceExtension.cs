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

namespace SF.Core.Events
{
	
	public static class EventSourceExtension
    {
		public static IEventObservable GetObservable(this ISourceResolver Resolver, string Source, string Type)
		{
			return Resolver.GetSource(Source).GetObservable(Type);
		}
		public static IDisposable Subscribe(this ISourceResolver Resolver, string Source,string Type, Func<object,Task> Callback)
		{
			return Resolver.GetObservable(Source,Type)
				.Subscribe(Callback);
		}
		public static IDisposable Subscribe(this ISourceResolver Resolver, string Source, Func<object, Task> Callback)
		{
			return Resolver.Subscribe(Source, null, Callback);
		}

		public static IDisposable Filter(
			this ISourceResolver Resolver,
			string Source,
			string Type,
			IEventEmitter Emiter,
			Func<object, object> Filter,
            bool OutputSyncMode=false
			)
		{
			return Resolver.Subscribe(
                Source, 
                Type, 
                e =>
                {
                    var re = Filter(e);
                    if (re == null)
                        return Task.CompletedTask;
                    return Emiter.Emit(re, OutputSyncMode);
                }
                );
		}

	}

}
