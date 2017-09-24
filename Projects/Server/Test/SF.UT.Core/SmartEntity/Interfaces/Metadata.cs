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

namespace SF.Entities.Smart
{
	public interface IAttribute
	{
		string Name { get; }
		IReadOnlyDictionary<string,object> Values { get; }		
	}
	public interface IMetaItem
	{
		string Name { get; }
		IReadOnlyList<IAttribute> Attributes { get; }
	}
	public enum PropertyMode
	{
		Value,
		SingleRelation,
		MultipleRelation
	}
	public interface IProperty : IMetaItem
	{
		IType Type { get; }
		PropertyMode Mode{ get; }
	}
	public interface IType : IMetaItem
	{
		
	}
	public interface IValueMapper
	{
		Type DataType { get; }
		Type TempType { get; }
		Type ModelType { get; }

		Expression DataValueToTempValue(Expression DataValue);
		Expression TempValueToDataValue(Expression TempValue);
		Expression TempValueToModelValue(Expression TempValue);
		Expression ModelValueToTempValue(Expression ModelValue);
	}

	public interface IValueTypeProvider
	{
		IValueMapper DetailValueMapper { get; }
		IValueMapper SummaryValueMapper { get; }
		IValueMapper EditableValueMapper { get; }
	}

	public interface IValueType : IType
	{
		IValueTypeProvider Provider { get; }
	}
	
	public interface IEntityType : IType
	{
		IEntityType BaseType { get; }
		IReadOnlyList<IProperty> Properties { get; }
	}
	public interface IMetadataCollection 
	{
		IReadOnlyList<IEntityType> Entities { get; }
		IReadOnlyDictionary<string, IType> Types { get; }
	}
}
