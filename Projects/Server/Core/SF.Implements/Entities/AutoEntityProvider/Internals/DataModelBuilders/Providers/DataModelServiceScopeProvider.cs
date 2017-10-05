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

	public class DataModelServiceScopeProvider : IDataModelTypeBuildProvider
	{
		public int Priority => 100;
		public static string RelatedServiceIdName { get; } = "_RelatedServiceId";

		public void AfterBuildType(IDataModelBuildContext Context, TypeExpression Type, IEntityType Entity)
		{
			if (!Entity.Attributes?.Any(a => a.Name == typeof(ServiceScopeEnabledAttribute).FullName)??false)
				return;

			var prop = Type.Properties.FirstOrDefault(p => p.Name == RelatedServiceIdName);
			if (prop != null)
				throw new InvalidOperationException($"������������ֶ�ʱ���󣬶������Ѷ�����Ϊ{RelatedServiceIdName}");

			prop= new PropertyExpression(
				RelatedServiceIdName,
				new SystemTypeReference(typeof(long?)),
				PropertyAttributes.None
				);

			prop.CustomAttributes.Add(
				new CustomAttributeExpression(
					typeof(IndexAttribute).GetConstructor(Array.Empty<Type>())
					)
				);
			Type.Properties.Add(prop);
		}

		public void BeforeBuildType(IDataModelBuildContext Context, TypeExpression Type, IEntityType Entity)
		{

		}
	}
}
