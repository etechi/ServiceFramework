using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Metadata;
using SF.Entities;
using SF.Entities.AutoEntityProvider;
using SF.Entities.AutoEntityProvider.Internals;
using SF.Data;

namespace SF.Core.ServiceManagement
{
	
	public static class AutoEntityProviderDICollectionExtension
	{
		class DataModuleSource : SF.Data.IEntityDataModelSource
		{
			 public EntityDataModels DataModels { get; set; }
		}
		static IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument> CreateEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(
			IServiceProvider sp)
			where TEntityDetail : class, IEntityWithId<TKey>
		   where TEntitySummary : class, IEntityWithId<TKey>
		   where TEntityEditable : class, IEntityWithId<TKey>
		   where TKey : IEquatable<TKey>
		   where TQueryArgument : IQueryArgument<TKey>
		{
			var f = sp.Resolve<DataSetAutoEntityProviderFactory>();
			return f.Create<TKey,TEntityDetail,TEntitySummary,TEntityEditable,TQueryArgument>(sp);
		}

		public static IServiceCollection AddAutoEntityService(this IServiceCollection sc)
		{
			sc.AddTransient<SystemTypeMetadataBuilder>();
			sc.AddTransient<DataModelTypeBuilder>();
			sc.AddSingleton<DataSetAutoEntityProviderFactory>();
			sc.AddSingleton<IValueTypeResolver, ValueTypeResolver>();

			sc.AddSingleton<IMetadataCollection>(sp =>
				sp.Resolve<SystemTypeMetadataBuilder>().Build()
				);
			
			sc.AddTransient(
				typeof(IDataSetAutoEntityProvider<, , , , >), 
				typeof(ServiceProviderBuilder).GetMethodExt(nameof(CreateEntityProvider), typeof(IServiceProvider))
				);

			sc.AddSingleton<SF.Data.IEntityDataModelSource>(
				sp =>
					new DataModuleSource
					{
						DataModels = new EntityDataModels(
							sp.Resolve<DataModelTypeBuilder>().Build(),
							string.Empty
							)
					}
				);

			return sc;
		}
	}
}
