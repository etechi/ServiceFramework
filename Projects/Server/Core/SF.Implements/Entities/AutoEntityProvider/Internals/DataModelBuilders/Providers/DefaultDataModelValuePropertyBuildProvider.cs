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

namespace SF.Entities.AutoEntityProvider.Internals.DataModelBuilders.Providers
{
	public class DefaultDataModelValuePropertyBuildProvider : IDataModelPropertyBuildProvider
	{
		public int Priority => 0;
		IDataModelTypeMapper[] DataModelTypeMappers { get; }
		NamedServiceResolver<IDataModelAttributeGenerator> DataModelAttributeGeneratorResolver { get; }

		public DefaultDataModelValuePropertyBuildProvider(
			IEnumerable<IDataModelTypeMapper> DataModelTypeMappers,
			NamedServiceResolver<IDataModelAttributeGenerator>  DataModelAttributeGeneratorResolver
			)
		{
			this.DataModelTypeMappers = DataModelTypeMappers.OrderBy(p=>p.Priority).ToArray();
			this.DataModelAttributeGeneratorResolver = DataModelAttributeGeneratorResolver;
		}
		
		public PropertyExpression AfterBuildProperty(IDataModelBuildContext Context, TypeExpression Type, PropertyExpression Property, IEntityType EntityType, IProperty EntityProperty)
		{
			return Property;
		}

		public PropertyExpression BeforeBuildProperty(IDataModelBuildContext Context, TypeExpression Type, PropertyExpression Property, IEntityType EntityType, IProperty EntityProperty)
		{
			if (EntityProperty.Mode != PropertyMode.Value)
				return Property;

			var sysType = ((IValueType)EntityProperty.Type).SysType;
			var re = DataModelTypeMappers
				.Select(p => p.MapType(EntityType, EntityProperty, sysType))
				.FirstOrDefault(t => t != null);
			return new PropertyExpression(
				EntityProperty.Name,
				new SystemTypeReference(re?.Type ?? sysType),
				PropertyAttributes.None,
				DataModelAttributeGeneratorResolver.ToExpressions(EntityProperty.Attributes)
				);
		}
	}
}
