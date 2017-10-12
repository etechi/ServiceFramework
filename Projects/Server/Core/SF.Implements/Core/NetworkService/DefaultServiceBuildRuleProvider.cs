using System;
using System.Linq;
using System.Reflection;
using SF.Metadata.Models;
using System.Collections.Generic;
using SF.Core.ServiceManagement;
using SF.Entities;
using SF.Metadata;

namespace SF.Core.NetworkService
{
	public class DefaultServiceBuildRuleProvider : IServiceBuildRuleProvider
	{
		IServiceProvider ServiceProvider { get; }
		public DefaultServiceBuildRuleProvider(IServiceProvider ServiceProvider)
		{
			this.ServiceProvider = ServiceProvider;
		}


		class MethodComparer : IEqualityComparer<MethodInfo>
		{
			public static MethodComparer Instance { get; } = new MethodComparer();

			public bool Equals(MethodInfo x, MethodInfo y)
			{
				if (x == y) return true;
				if (x.Name != y.Name) return false;
				var xps = x.GetParameters();
				var yps = y.GetParameters();
				if (xps.Length != yps.Length) return false;
				if (xps.Zip(yps, (xp, yp) => xp.ParameterType != yp.ParameterType).Any()) return false;
				return true;
			}

			public int GetHashCode(MethodInfo obj)
			{
				return obj.GetHashCode();
			}
		}

		public IEnumerable<MethodInfo> GetServiceMethods(System.Type type)
		{
			return GetServiceMethods(type, new HashSet<MethodInfo>(MethodComparer.Instance));
		}
		public IEnumerable<ParameterInfo> GetMethodParameters(System.Reflection.MethodInfo method)
		{
			return method.GetParameters().Where(p => p.ParameterType != typeof(Entities.Paging));

		}
		IEnumerable<MethodInfo> GetServiceMethods(System.Type type, HashSet<MethodInfo> methods)
		{
			foreach (var method in type.
				GetMethods(
				BindingFlags.Public |
				BindingFlags.Instance |
				BindingFlags.FlattenHierarchy
				))
			{
				if (methods.Add(method))
					yield return method;
			}
			if (!type.IsInterfaceType())
				yield break;
			foreach (var it in type.GetInterfaces())
				foreach (var m in GetServiceMethods(it, methods))
					yield return m;
		}
		public static bool IsHeavyParameter(ParameterInfo p)
		{
			var t = p.ParameterType;
			if (t == typeof(Entities.Paging))
				return false;
			switch (t.GetTypeCode())
			{
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.DateTime:
				case TypeCode.String:
					return false;
				case TypeCode.Object:
					if (t.IsArray)
						return true;
					if (t.IsValue())
						return false;
					return true;
				default:
					return false;
			}
		}
        public ParameterInfo DetectHeavyParameter(MethodInfo method)
		{
			return method.GetParameters().LastOrDefault(p => IsHeavyParameter(p));
		}

		public string FormatServiceName(System.Type type)
		{
			var name = type.GetTypeName(false);
			var b = 0;
			if (type.IsInterfaceType() && name.Length > 2 && name[0] == 'I')
				b++;
			var e = name.Length;
			if (name.Length - b > 7 && name.EndsWith("Service"))
				e -= 7;
			else if (name.Length - b > 10 && name.EndsWith("Controller"))
				e -= 10;
			if(b>0 || e<name.Length)
				name = name.Substring(b, e - b);

			return name;
		}
		public string FormatMethodName(System.Reflection.MethodInfo Method)
		{
			var name = Method.Name;
			if (name.EndsWith("Async"))
				name = name.Substring(0, name.Length - 5);
			return name;
		}

		public IMetadataAttributeValuesProvider TryGetAttributeValuesProvider(System.Attribute attr)
		{
			var type = typeof(IMetadataAttributeValuesProvider<>).MakeGenericType(attr.GetType());
			return ServiceProvider.GetService(type) as IMetadataAttributeValuesProvider;
		}
	}
}
