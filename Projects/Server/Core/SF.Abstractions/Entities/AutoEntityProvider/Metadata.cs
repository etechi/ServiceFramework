using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace SF.Entities.AutoEntityProvider
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
		Type SysType { get; }
	}
	
	public interface IEntityType : IType
	{
		string FullName { get; }
		string Namespace{ get; }
		IReadOnlyList<IProperty> Properties { get; }
	}
	public interface IMetadataCollection 
	{
		IReadOnlyDictionary<string, IEntityType> EntityTypes { get; }
		IReadOnlyDictionary<Type,IEntityType> EntityTypesByType { get; }
	}
}
