using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;
using SF.Core.ServiceManagement;
using SF.Entities.AutoEntityProvider.Internals.ValueTypes;

namespace SF.Entities.AutoEntityProvider.Internals
{
	
	
	public class ValueTypeResolver : IValueTypeResolver
	{
		System.Collections.Concurrent.ConcurrentDictionary<string, IValueType> Types = new System.Collections.Concurrent.ConcurrentDictionary<string, IValueType>();
		public ValueTypeResolver(IEnumerable<IValueType> ValueTypes)
		{
			foreach (var t in ValueTypes)
				Types.TryAdd(t.SysType.FullName, t);
		}
		public IValueType Resolve(string TypeName,string PropName, Type SystemType, IReadOnlyList<IAttribute> Attributes)
		{
			var key = SystemType.FullName;
			if (Types.TryGetValue(key, out var vt))
				return vt;
			if (SystemType.IsEnumType())
			{
				vt = (IValueType)Activator.CreateInstance(typeof(PrimitiveValueType<>).MakeGenericType(SystemType));
				return Types.GetOrAdd(key, vt);
			}
			else if(
				SystemType.IsGeneric() && 
				SystemType.GetGenericTypeDefinition() == typeof(Nullable<>) && 
				Types.ContainsKey(SystemType.GenericTypeArguments[0].FullName)
				)
			{
				vt = (IValueType)Activator.CreateInstance(typeof(PrimitiveValueType<>).MakeGenericType(SystemType));
				return Types.GetOrAdd(key, vt);
			}
			throw new NotSupportedException($"找不到数据类型{TypeName}.{PropName} 系统类型:{SystemType} 特性:{Attributes?.Select(a => a.Name + " " + a.Values?.Select(p => p.Key + ":" + p.Value)).Join(";")}");
		}
	}
}
