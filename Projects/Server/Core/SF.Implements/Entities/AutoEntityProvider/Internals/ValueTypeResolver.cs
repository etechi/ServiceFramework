using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;
using SF.Core.ServiceManagement;

namespace SF.Entities.AutoEntityProvider.Internals
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
