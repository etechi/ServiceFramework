using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using Xunit;
using SF.Applications;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.ServiceFeatures;
using System.Threading.Tasks;
using System.Linq;

namespace SF.Entities.Smart.Internals.ValueTypes
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
