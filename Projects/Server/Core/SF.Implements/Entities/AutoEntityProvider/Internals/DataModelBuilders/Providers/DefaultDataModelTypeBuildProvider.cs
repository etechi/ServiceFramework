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


	public class DefaultDataModelTypeBuildProvider : IDataModelTypeBuildProvider
	{
		IDataModelPropertyBuildProvider[] PropertyBuildProviders { get; }
		IDataModelPropertyBuildProvider[] InversePropertyBuildProviders { get; }
		public DefaultDataModelTypeBuildProvider(IEnumerable<IDataModelPropertyBuildProvider> DataModelPropertyBuildProviders)
		{
			PropertyBuildProviders = DataModelPropertyBuildProviders.OrderBy(p => p.Priority).ToArray();
			InversePropertyBuildProviders = this.PropertyBuildProviders.Reverse().ToArray();
		}

		public int Priority => 0;

		public void BeforeBuildType(IDataModelBuildContext Context, TypeExpression Type, IEntityType Entity)
		{
			foreach (var prop in Entity.Properties)
				foreach (var provider in PropertyBuildProviders)
				{
					var idx = Type.Properties.IndexOf(tp => tp.Name == prop.Name);
					var op = idx == -1 ? null : Type.Properties[idx];
					var np= provider.BeforeBuildProperty(Context, Type, op, Entity, prop);
					if (np == op)
						continue;
					if (idx == -1)
					{
						if (np != null)
							Type.Properties.Add(np);
					}
					else
						Type.Properties[idx] = np;
				}
		}

		public void AfterBuildType(IDataModelBuildContext Context, TypeExpression Type, IEntityType Entity)
		{
			foreach (var prop in Entity.Properties)
				foreach (var provider in InversePropertyBuildProviders)
				{
					var idx = Type.Properties.IndexOf(tp => tp.Name == prop.Name);
					var op = idx == -1 ? null : Type.Properties[idx];
					var np = provider.AfterBuildProperty(Context, Type, op, Entity, prop);
					if (np == op)
						continue;
					if (idx == -1)
					{
						if (np != null)
							Type.Properties.Add(np);
					}
					else
						Type.Properties[idx] = np;
				}
		}
	}
}
