using SF.Entities.AutoEntityProvider.Internals;
using System;
using System.Collections.Generic;
namespace SF.Entities.AutoEntityProvider
{
	public class DataSetAutoEntityProviderFactory
	{
		System.Collections.Concurrent.ConcurrentDictionary<Type, DataSetAutoEntityProviderSetting> Creators = 
			new System.Collections.Concurrent.ConcurrentDictionary<Type, DataSetAutoEntityProviderSetting>(); 
		IMetadataCollection Metas { get; }
		IDataModelTypeCollection DataModelTypeCollection { get; }
		public DataSetAutoEntityProviderFactory(IMetadataCollection Metas, IDataModelTypeCollection DataModelTypeCollection)
		{
			this.Metas = Metas;
			this.DataModelTypeCollection = DataModelTypeCollection;
		}

		void ValidateEntityTypes(IEntityType entity, params Type[] types)
		{
			foreach (var type in types)
			{
				var ne = metas.EntityTypesByType.Get(type);
				if (ne == null)
					throw new ArgumentException($"自动化实体类型库中找不到类型{type}对应的实体");

				if (ne != entity)
					throw new ArgumentException($"类型{type}对应的实体{ne.FullName}和类型{typeof(TEntityDetail)}对应的实体{entity.FullName}不一致");
			}
		}
		

		(Type TempDetail, Type TempSummary, Type DataModel) TempTypeDetect(
			Type TKey, Type TEntityDetail, Type TEntitySummary, Type TEntityEditable, Type TQueryArgument
			)
		{
			var entity = Metas.EntityTypesByType.Get(TEntityDetail);
			if (entity == null)
				throw new ArgumentException($"自动化实体类型库中找不到类型{TEntityDetail}对应的实体");
				ValidateEntityTypes(
					entity,
					TEntitySummary,
					TEntityEditable
					);
				this.DataModelTypeCollection.TryGetValue()
		}
		public IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>  
			Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(IServiceProvider sp)
			where TEntityDetail : class, IEntityWithId<TKey>
			where TEntitySummary : class, IEntityWithId<TKey>
			where TEntityEditable : class, IEntityWithId<TKey>
			where TKey : IEquatable<TKey>
			where TQueryArgument : IQueryArgument<TKey>
		{
			var tDetail = typeof(TEntityDetail);
			if (!Creators.TryGetValue(tDetail, out var setting))
			{
				var tKey = typeof(TKey);
				var tSummary = typeof(TEntitySummary);
				var tEditable = typeof(TEntityEditable);
				var tQueryArg = typeof(TQueryArgument);
				var tmpTypes = TempTypeDetect(tKey, tDetail, tSummary, tEditable, tQueryArg);
				setting = Creators.GetOrAdd(
					tDetail,
					(DataSetAutoEntityProviderSetting)Activator.CreateInstance(
						typeof(DataSetAutoEntityProviderSetting<,,,,,,,>).MakeGenericType(
						tKey, 
						tDetail,
						tmpTypes.TempDetail,
						tSummary,
						tmpTypes.TempSummary,
						tEditable,
						tQueryArg
						),
						Metas
					)
				);
			}
			return (IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>)setting.FuncCreateProvider.Value(sp);
		}
	}


}
