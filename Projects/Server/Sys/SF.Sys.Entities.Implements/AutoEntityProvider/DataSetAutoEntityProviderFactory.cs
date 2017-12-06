#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Sys.Collections.Generic;
using SF.Sys.Reflection;
using SF.Sys.Services;
using System;
using System.Linq;
using System.Reflection;

namespace SF.Sys.Entities.AutoEntityProvider
{
	public class DataSetAutoEntityProviderFactory : IDataSetAutoEntityProviderFactory
	{
		IDataSetAutoEntityProviderCache DataSetAutoEntityProviderCache { get; }
		IServiceProvider ServiceProvider { get; }
		long? CurServiceScopeId { get; }
		public DataSetAutoEntityProviderFactory(
			IDataSetAutoEntityProviderCache DataSetAutoEntityProviderCache,
			IServiceProvider ServiceProvider
			)
		{
			this.DataSetAutoEntityProviderCache = DataSetAutoEntityProviderCache;
			this.ServiceProvider = ServiceProvider;
			this.CurServiceScopeId = ServiceProvider.Resolver().CurrentServiceId;
		}

		public IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>()
			where TQueryArgument:IPagingArgument
		{
			using (this.ServiceProvider.Resolver().WithScopeService(CurServiceScopeId))
				return DataSetAutoEntityProviderCache.Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(ServiceProvider);
		}
		public IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		   Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument, TDataModel>()
			where TDataModel : class
			where TQueryArgument : IPagingArgument
		{
			using (this.ServiceProvider.Resolver().WithScopeService(CurServiceScopeId))
				return DataSetAutoEntityProviderCache.Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument,TDataModel>(ServiceProvider);
		}
	}
	public class DataSetAutoEntityProviderCache : IDataSetAutoEntityProviderCache
	{
		System.Collections.Concurrent.ConcurrentDictionary<Type, Lazy<IDataSetAutoEntityProviderSetting>> Creators =
			new System.Collections.Concurrent.ConcurrentDictionary<Type, Lazy<IDataSetAutoEntityProviderSetting>>();
		IMetadataCollection Metas { get; }
		IDataModelTypeCollection DataModelTypeCollection { get; }
		public DataSetAutoEntityProviderCache(
			IMetadataCollection Metas,
			IDataModelTypeCollection DataModelTypeCollection
			)
		{
			this.Metas = Metas;
			this.DataModelTypeCollection = DataModelTypeCollection;
		}

		void ValidateEntityTypes(Type TEntityDetail, IEntityType entity, params Type[] types)
		{
			foreach (var type in types)
			{
				var ne = Metas.EntityTypesByType.Get(type);
				if (ne == null)
					throw new ArgumentException($"自动化实体类型库中找不到类型{type}对应的实体");

				if (ne != entity)
					throw new ArgumentException($"类型{type}对应的实体{ne.Name}和类型{TEntityDetail}对应的实体{entity.Name}不一致");
			}
		}

		static IDataSetAutoEntityProviderSetting
			NewSetting<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument, TDataModel>()
			where TDataModel : class,new()
			where TQueryArgument : IPagingArgument
		{
			return new DataSetAutoEntityProviderSetting<
				TKey,
				TEntityDetail, 
				TEntitySummary, 
				TEntityEditable, 
				TQueryArgument, 
				TDataModel
				>(
				);
			}

		IDataSetAutoEntityProviderSetting CreateSetting<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(Type DataModelType)
		{
			var tKey = typeof(TKey);
			var tDetail = typeof(TEntityDetail);
			var tSummary = typeof(TEntitySummary);
			var tEditable = typeof(TEntityEditable);
			var tQueryArg = typeof(TQueryArgument);

			var entity = Metas.EntityTypesByType.Get(tDetail);
			if (entity == null)
				throw new ArgumentException($"自动化实体类型库中找不到类型{tDetail}对应的实体");
			ValidateEntityTypes(
				tDetail,
				entity,
				tSummary,
				tEditable
				);
			var dataType = DataModelType?? DataModelTypeCollection.Get(entity.Name);

			var newSetting = typeof(DataSetAutoEntityProviderCache).GetMethods(
				nameof(DataSetAutoEntityProviderCache.NewSetting),
				BindingFlags.Static | BindingFlags.NonPublic
				).Single();

			return (IDataSetAutoEntityProviderSetting)newSetting.MakeGenericMethod(
			   tKey,
			  tDetail,
			  tSummary,
			  tEditable,
			  tQueryArg,
			  dataType
			  ).Invoke(
				null,
				Array.Empty<object>()
			);
		}
		public IDataSetAutoEntityProvider<TKey,TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>  
			Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(IServiceProvider sp)
			where TQueryArgument : IPagingArgument
		{
			var tDetail = typeof(TEntityDetail);
			if (!Creators.TryGetValue(tDetail, out var setting))
			{
				setting = Creators.GetOrAdd(
					tDetail, 
					new Lazy<IDataSetAutoEntityProviderSetting>(() =>
					   CreateSetting<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(null)
					)
				);
			}
			return (IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>)setting.Value.FuncCreateProvider.Value(sp);
		}
		public IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		   Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument,TDataModel>(IServiceProvider sp)
			where TDataModel:class
			where TQueryArgument : IPagingArgument
		{
			var tDetail = typeof(TEntityDetail);
			if (!Creators.TryGetValue(tDetail, out var setting))
			{
				setting = Creators.GetOrAdd(
					tDetail,
					new Lazy<IDataSetAutoEntityProviderSetting>(() =>
					   CreateSetting<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(typeof(TDataModel))
					)
				);
			}
			return (IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>)setting.Value.FuncCreateProvider.Value(sp);
		}
	}


}
