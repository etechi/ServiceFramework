using System;
using System.Linq;
using System.Reflection;
using SF.Metadata.Models;
using SF.Reflection;
namespace SF.Services.Metadata
{
	public class DefaultServiceBuildRuleProvider : IServiceBuildRuleProvider
	{
		public static bool IsHeavyParameter(ParameterInfo p)
		{
			var t = p.ParameterType;
			switch (System.Type.GetTypeCode(t))
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
			var name = type.Name;
			var b = 0;
			if (type.IsInterfaceType() && name.Length > 2 && name[0] == 'I')
				b++;
			var e = name.Length;
			if (name.Length - b > 7 && name.EndsWith("Service"))
				e -= 7;
			else if (name.Length - b > 10 && name.EndsWith("Controller"))
				e -= 10;
			name = name.Substring(b, e - b);
			return name;
		}
	}
}
