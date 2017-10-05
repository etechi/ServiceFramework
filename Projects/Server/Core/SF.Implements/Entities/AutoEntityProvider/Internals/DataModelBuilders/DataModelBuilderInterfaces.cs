using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;
using System.Reflection.Emit;
using SF.Core.ServiceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq.TypeExpressions;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelBuilders
{
	
	public class TypeMapResult
	{
		public Type Type { get; set; }
		public IReadOnlyList<IAttribute> Attributes { get; set; }
	}
	public interface IDataModelTypeMapper
	{
		int Priority { get; }
		TypeMapResult MapType(IEntityType EntityType, IProperty Property, Type Type);
	}
	public interface IDataModelAttributeGenerator
	{
		CustomAttributeExpression Generate(IAttribute Attr);
	}
	public interface IDataModelBuildContext
	{
		Dictionary<string, TypeExpression> TypeExpressions{ get; }
		IMetadataCollection Metadata { get; }
	}
	public interface IDataModelPropertyBuildProvider
	{
		int Priority { get; }
		PropertyExpression BeforeBuildProperty(
			IDataModelBuildContext Context, 
			TypeExpression Type, 
			PropertyExpression Property, 
			IEntityType EntityType,
			IProperty EntityProperty
			);
		PropertyExpression AfterBuildProperty(
			IDataModelBuildContext Context, 
			TypeExpression Type, 
			PropertyExpression Property, 
			IEntityType EntityType, 
			IProperty EntityProperty
			);
	}
	public interface IDataModelTypeBuildProvider
	{
		int Priority { get; }
		void BeforeBuildType(
			IDataModelBuildContext Context, 
			TypeExpression Type, 
			IEntityType Entity
			);
		void AfterBuildType(
			IDataModelBuildContext Context, 
			TypeExpression Type, 
			IEntityType Entity
			);
	}
	public interface IDataModelBuildProvider
	{
		int Priority { get; }
		void BeforeBuildModel(IDataModelBuildContext Context);
		void AfterBuildModel(IDataModelBuildContext Context);
	}
	

}
