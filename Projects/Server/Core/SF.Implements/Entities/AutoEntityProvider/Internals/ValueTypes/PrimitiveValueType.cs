using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SF.Entities.AutoEntityProvider.Internals.ValueTypes
{
	public class PrimitiveValueType<T> : IValueType
	{

		public Type DataType => ModelType;

		public Type TempType => ModelType;

		public Type ModelType { get; } = typeof(T);

		public string Name => ModelType.FullName;

		public IReadOnlyList<IAttribute> Attributes => Array.Empty<IAttribute>();

		public IValueTypeProvider Provider => throw new NotImplementedException();

		public Type SysType => ModelType;

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
