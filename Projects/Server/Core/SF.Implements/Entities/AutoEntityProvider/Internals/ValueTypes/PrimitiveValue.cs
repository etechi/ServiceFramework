using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SF.Entities.AutoEntityProvider.Internals.ValueTypes
{
	public class PrimitiveType : IValueType
	{
		public PrimitiveType(Type Type)
		{
			ModelType = Type;
		}

		public Type DataType => ModelType;

		public Type TempType => ModelType;

		public Type ModelType { get; }

		public string Name => ModelType.FullName;

		public IReadOnlyList<IAttribute> Attributes => Array.Empty<IAttribute>();

		public IValueTypeProvider Provider => throw new NotImplementedException();

		public Type SysType => throw new NotImplementedException();

		public Expression DataValueToTempValue(Expression DataValue)
		{
			return DataValue;
		}

		public Expression TempValueToDataValue(Expression TempValue)
		{
			return TempValue;
		}
	}

}
