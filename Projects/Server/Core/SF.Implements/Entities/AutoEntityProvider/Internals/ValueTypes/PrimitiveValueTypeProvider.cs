using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace SF.Entities.AutoEntityProvider.Internals.ValueTypes
{
	public class PrimitiveValueTypeProvider : IValueTypeProvider
	{
		class PrimitiveValueType : IValueType
		{
			public PrimitiveValueType(Type Type)
			{
				this.SysType = Type;
				this.Attributes = EntityAttribute.GetAttributes(Type).ToArray();
			}
			public Type SysType { get; }

			public string Name => SysType.FullName;

			public IReadOnlyList<IAttribute> Attributes { get; }
		}
		public PrimitiveValueTypeProvider()
		{
		}

		public int Priority => 0;
		static bool IsPrimitiveType(Type SystemType)
		{
			switch (Type.GetTypeCode(SystemType))
			{
				case TypeCode.Boolean:
				case TypeCode.Byte:
				case TypeCode.Char:
				case TypeCode.DateTime:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.String:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return true;
			}
			if (SystemType.IsEnum)
				return true;
			return false;
		}
		public IValueType DetectValueType(string TypeName, string PropName, Type SystemType, IReadOnlyList<IAttribute> Attributes)
		{
			if (IsPrimitiveType(SystemType))
				return new PrimitiveValueType(SystemType);
			if (SystemType.IsGenericType && 
				SystemType.GetGenericTypeDefinition()==typeof(Nullable<>) &&
				IsPrimitiveType(SystemType.GenericTypeArguments[0])
				)
				return new PrimitiveValueType(SystemType);
			return null;
		}

		
	}
}
