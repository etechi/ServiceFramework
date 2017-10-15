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

using SF.Core.ServiceManagement;
using SF.Entities.AutoEntityProvider.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SF.Entities.AutoEntityProvider
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
		{
			using (this.ServiceProvider.Resolver().WithScopeService(CurServiceScopeId))
				return DataSetAutoEntityProviderCache.Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(ServiceProvider);
		}
		public IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		   Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument, TDataModel>()
			where TDataModel : class
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
		IEntityPropertyQueryConverterProvider[] PropertyQueryConverterProviders { get; }
		IQueryFilterProvider[] QueryFilterProviders { get; }
		NamedServiceResolver<IEntityPropertyModifier> EntityModifierValueConverterResolver { get; }
		IEntityModifierBuilder EntityModifierBuilder { get; }
		public DataSetAutoEntityProviderCache(
			IMetadataCollection Metas,
			IDataModelTypeCollection DataModelTypeCollection,
			IEnumerable<IEntityPropertyQueryConverterProvider> PropertyQueryConverterProviders,
			IEntityModifierBuilder EntityModifierBuilder,
			IEnumerable<IQueryFilterProvider> QueryFilterProviders
			)
		{
			this.Metas = Metas;
			this.DataModelTypeCollection = DataModelTypeCollection;
			this.PropertyQueryConverterProviders = PropertyQueryConverterProviders.OrderBy(p=>p.Property).ToArray();
			this.EntityModifierValueConverterResolver = EntityModifierValueConverterResolver;
			this.EntityModifierBuilder = EntityModifierBuilder;
			this.QueryFilterProviders = QueryFilterProviders.ToArray();
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
			NewSetting<TKey, TEntityDetail, TEntityDetailTemp, TEntitySummary, TEntitySummaryTemp, TEntityEditable, TEntityEditableTemp, TQueryArgument, TDataModel>(
			QueryResultBuildHelper DetailQueryResultBuildHelper,
			QueryResultBuildHelper SummaryQueryResultBuildHelper,
			QueryResultBuildHelper EditableQueryResultBuildHelper,
			IEntityModifierBuilder EntityModifierBuilder,
			IQueryFilterProvider[] QueryFilterProviders
			)
			where TDataModel : class,new()
			{
			return new DataSetAutoEntityProviderSetting<
				TKey,
				TEntityDetail, 
				TEntityDetailTemp, 
				TEntitySummary, 
				TEntitySummaryTemp, 
				TEntityEditable, 
				TEntityEditableTemp, 
				TQueryArgument, 
				TDataModel
				>(
				(IQueryResultBuildHelper<TDataModel, TEntityDetailTemp, TEntityDetail>)DetailQueryResultBuildHelper,
				(IQueryResultBuildHelper<TDataModel, TEntitySummaryTemp, TEntitySummary>)SummaryQueryResultBuildHelper,
				(IQueryResultBuildHelper<TDataModel, TEntityEditableTemp, TEntityEditable>)EditableQueryResultBuildHelper,
				EntityModifierBuilder.GetEntityModifier<TEntityEditable,TDataModel>(DataActionType.Create),
				EntityModifierBuilder.GetEntityModifier<TEntityEditable, TDataModel>(DataActionType.Update),
				EntityModifierBuilder.GetEntityModifier<TEntityEditable, TDataModel>(DataActionType.Delete),
				new QueryFilterBuildHelper(QueryFilterProviders).Build<TDataModel,TQueryArgument>()
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

			var detailHelper = new QueryResultBuildHelperCreator(dataType, tDetail, PropertyQueryConverterProviders).Build();
			var summaryHelper = new QueryResultBuildHelperCreator(dataType, tSummary, PropertyQueryConverterProviders).Build();
			var editableHelper = new QueryResultBuildHelperCreator(dataType, tEditable, PropertyQueryConverterProviders).Build();

			var newSetting = typeof(DataSetAutoEntityProviderCache).GetMethods(
				nameof(DataSetAutoEntityProviderCache.NewSetting),
				BindingFlags.Static | BindingFlags.NonPublic
				).Single();

			return (IDataSetAutoEntityProviderSetting)newSetting.MakeGenericMethod(
			   tKey,
			  tDetail,
			  detailHelper.TempType,
			  tSummary,
			  summaryHelper.TempType,
			  tEditable,
			  editableHelper.TempType,
			  tQueryArg,
			  dataType
			  ).Invoke(
				null,
				new object[] {
						detailHelper,
						summaryHelper,
						editableHelper,
						EntityModifierBuilder,
						QueryFilterProviders
			}
			);
		}
		public IDataSetAutoEntityProvider<TKey,TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>  
			Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(IServiceProvider sp)
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
