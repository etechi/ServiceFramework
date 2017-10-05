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
	public class DataModelEntityIdentPropertyBuildProvider : IDataModelPropertyBuildProvider
	{
		public int Priority => 100;

		public PropertyExpression AfterBuildProperty(IDataModelBuildContext Context, TypeExpression Type, PropertyExpression Property, IEntityType EntityType, IProperty EntityProperty)
		{
			if(EntityProperty.Attributes?.Any(a=>a.Name==typeof(EntityIdentAttribute).FullName) ?? false)
			{
				DataModelBuildHelper.EnsureSingleFieldIndex(
					Type,
					Property,
					EntityType,
					EntityProperty
					);
			}
			return Property;
		}

		public PropertyExpression BeforeBuildProperty(IDataModelBuildContext Context, TypeExpression Type, PropertyExpression Property, IEntityType EntityType, IProperty EntityProperty)
		{
			return Property;
		}
	}
}
