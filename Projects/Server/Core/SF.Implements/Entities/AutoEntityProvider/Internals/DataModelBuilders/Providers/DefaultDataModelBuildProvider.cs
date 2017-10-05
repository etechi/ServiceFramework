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


	public class DefaultDataModelBuildProvider : IDataModelBuildProvider
	{
		IDataModelTypeBuildProvider[] DataModelTypeBuildProviders { get; }
		public DefaultDataModelBuildProvider(IEnumerable<IDataModelTypeBuildProvider> DataModelTypeBuildProviders)
		{
			this.DataModelTypeBuildProviders = DataModelTypeBuildProviders.OrderBy(p => p.Priority).ToArray();
		}

		public int Priority => 0;

		public void AfterBuildModel(IDataModelBuildContext Context)
		{
			foreach (var type in Context.TypeExpressions)
				foreach (var provider in DataModelTypeBuildProviders)
					provider.AfterBuildType(Context, type.Value, Context.Metadata.EntityTypes[type.Key]);
		}

		public void BeforeBuildModel(IDataModelBuildContext Context)
		{
			foreach (var type in Context.TypeExpressions)
				foreach (var provider in DataModelTypeBuildProviders.Reverse())
					provider.BeforeBuildType(Context, type.Value, Context.Metadata.EntityTypes[type.Key]);
		}
	}
}
