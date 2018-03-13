using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Utils.TableExports
{

    public class DefaultFormatResolver : IFormatResolver
    {
        public static IFormatResolver Instance { get; } = new DefaultFormatResolver();
        System.Collections.Concurrent.ConcurrentDictionary<Type, EnumFormat> EnumFormats = new System.Collections.Concurrent.ConcurrentDictionary<Type, EnumFormat>();
        IFormat GetEnumFormat(Type type)
        {
            return EnumFormats.GetOrAdd(type, t => new EnumFormat(t));
        }
        public IFormat Resolve(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
                return (IFormat)Activator.CreateInstance(
                    typeof(NullableFormat<>).MakeGenericType(type),
                    Resolve(type)
                    );
            }

            if (type.IsEnum)
                return GetEnumFormat(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return BoolFormat.Instance;
                case TypeCode.Byte:
                    return NumberFormat.Instance;
                case TypeCode.Char:
                    return CommonFormat.Instance;
                case TypeCode.DateTime:
                    return DateTimeFormat.Instance;
                case TypeCode.DBNull:
                    return NullFormat.Instance;
                case TypeCode.Decimal:
                case TypeCode.Double:
                    return NumberFormat.Instance;
                case TypeCode.Empty:
                    return NullFormat.Instance;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                    return NumberFormat.Instance;
                case TypeCode.String:
                    return CommonFormat.Instance;
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return NumberFormat.Instance;
                case TypeCode.Object:
                default:
                    return CommonFormat.Instance;

            }
        }
    }
}
