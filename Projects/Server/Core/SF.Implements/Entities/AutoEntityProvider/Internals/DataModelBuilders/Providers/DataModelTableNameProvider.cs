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

	public class DataModelTableNameProvider : IDataModelTypeBuildProvider
	{
		public int Priority => 100;

		public void AfterBuildType(IDataModelBuildContext Context, TypeExpression Type, IEntityType Entity)
		{
			if (Type.CustomAttributes.Any(t => t.Constructor.ReflectedType == typeof(TableAttribute)))
				return;

			Type.CustomAttributes.Add(
				new CustomAttributeExpression(
				typeof(TableAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] {  Entity.Name }
				)
				);
		}

		public void BeforeBuildType(IDataModelBuildContext Context, TypeExpression Type, IEntityType Entity)
		{

		}
	}
}
