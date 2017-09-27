using System;

namespace SF.Entities.AutoEntityProvider
{
	public class DataSetAutoEntityProviderFactory
	{
		System.Collections.Concurrent.ConcurrentDictionary<(Type,Type,Type,Type,Type), DataSetAutoEntityProviderSetting> Creators = 
			new System.Collections.Concurrent.ConcurrentDictionary<(Type, Type, Type, Type, Type), DataSetAutoEntityProviderSetting>(); 
		IMetadataCollection Metas { get; }
		public DataSetAutoEntityProviderFactory(IMetadataCollection Metas)
		{
			this.Metas = Metas;
		}
		
		public IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>  
			Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(IServiceProvider sp)
			where TEntityDetail : class, IEntityWithId<TKey>
			where TEntitySummary : class, IEntityWithId<TKey>
			where TEntityEditable : class, IEntityWithId<TKey>
			where TKey : IEquatable<TKey>
			where TQueryArgument : IQueryArgument<TKey>
		{
			var key = (typeof(TKey), typeof(TEntityDetail), typeof(TEntitySummary), typeof(TEntityEditable), typeof(TQueryArgument));
			if (!Creators.TryGetValue(key, out var setting))
				setting = Creators.GetOrAdd(
					key,
					(DataSetAutoEntityProviderSetting)Activator.CreateInstance(
						typeof(DataSetAutoEntityProviderSetting<,,,,,,,>).MakeGenericType(
						typeof(TKey), typeof(TEntityDetail), typeof(TEntitySummary), typeof(TEntityEditable), typeof(TQueryArgument)
						),
						Metas
					)
				);
			return (IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>)setting.FuncCreateProvider.Value(sp);
		}
	}


}
