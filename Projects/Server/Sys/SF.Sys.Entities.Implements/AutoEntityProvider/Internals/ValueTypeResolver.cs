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

using SF.Sys.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SF.Sys.Entities.AutoEntityProvider.Internals
{


	public class ValueTypeResolver : IValueTypeResolver
	{
		IValueTypeProvider[] ValueTypeProviders { get; }
		System.Collections.Concurrent.ConcurrentDictionary<string, IValueType> Types = new System.Collections.Concurrent.ConcurrentDictionary<string, IValueType>();
		public ValueTypeResolver(IEnumerable<IValueTypeProvider> ValueTypeProviders)
		{
			this.ValueTypeProviders = ValueTypeProviders.OrderBy(p => p.Priority).ToArray();
			//foreach (var t in ValueTypes)
			//	Types.TryAdd(t.SysType.FullName, t);
		}
		public IValueType Resolve(string TypeName,string PropName, Type SystemType, IReadOnlyList<IAttribute> Attributes)
		{
			var key = SystemType.FullName;
			if (Types.TryGetValue(key, out var vt))
				return vt;
			vt = ValueTypeProviders.Select(p => p.DetectValueType(TypeName, PropName, SystemType, Attributes)).FirstOrDefault(p=>p!=null);
			if(vt==null)
				throw new NotSupportedException($"找不到数据类型{TypeName}.{PropName} 系统类型:{SystemType} 特性:{Attributes?.Select(a => a.Name + " " + a.Values?.Select(p => p.Key + ":" + p.Value)).Join(";")}");
			return Types.GetOrAdd(key, vt);
		}
	}
}
