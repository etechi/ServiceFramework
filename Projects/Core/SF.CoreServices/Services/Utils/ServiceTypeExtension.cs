using System;
using System.Linq;
using System.Reflection;
using SF.Reflection;
namespace SF.Services.Utils
{
	public static class ServiceTypeExtensions
    {
		public static bool IsHeavyParameter(this ParameterInfo p)
		{
			var t = p.ParameterType;
			switch (Type.GetTypeCode(t))
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
        public static ParameterInfo GetHeavyParameter(this MethodInfo method)
		{
			return method.GetParameters().LastOrDefault(p => p.IsHeavyParameter());

		}
	}
}
