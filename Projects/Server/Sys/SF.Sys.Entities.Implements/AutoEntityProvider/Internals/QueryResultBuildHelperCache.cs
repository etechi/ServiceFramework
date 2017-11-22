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

namespace SF.Sys.Entities.AutoEntityProvider.Internals
{

	public class QueryResultBuildHelperCache : IQueryResultBuildHelperCache
	{
		Lazy<IEntityPropertyQueryConverterProvider[]> PropertyQueryConverterProviders { get; }
		public QueryResultBuildHelperCache(Lazy<IEnumerable<IEntityPropertyQueryConverterProvider>> PropertyQueryConverterProviders)
		{
			this.PropertyQueryConverterProviders = new Lazy<IEntityPropertyQueryConverterProvider[]>(() => PropertyQueryConverterProviders.Value.OrderBy(p => p.Priority).ToArray());
		}
		System.Collections.Concurrent.ConcurrentDictionary<(Type, Type, QueryMode), object> Helpers { get; } 
			= new System.Collections.Concurrent.ConcurrentDictionary<(Type, Type, QueryMode), object>();

		public IQueryResultBuildHelper<E, R> GetHelper<E, R>(QueryMode Mode)
		{
			var key = (typeof(E), typeof(R), Mode);
			if (Helpers.TryGetValue(key, out var b))
				return (IQueryResultBuildHelper<E, R>)b;
			return (IQueryResultBuildHelper<E, R>)Helpers.GetOrAdd(
				key, 
				new QueryResultBuildHelperCreator(
					typeof(E),
					typeof(R),
					Mode,
					PropertyQueryConverterProviders.Value
					).Build()
				);
		}
	}
}
