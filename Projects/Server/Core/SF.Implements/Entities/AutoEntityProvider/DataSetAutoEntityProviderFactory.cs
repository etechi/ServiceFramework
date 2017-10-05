﻿using SF.Core.ServiceManagement;
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
		public DataSetAutoEntityProviderFactory(
			IDataSetAutoEntityProviderCache DataSetAutoEntityProviderCache,
			IServiceProvider ServiceProvider
			)
		{
			this.DataSetAutoEntityProviderCache = DataSetAutoEntityProviderCache;
			this.ServiceProvider = ServiceProvider;

		}

		public IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>()
		{
			return DataSetAutoEntityProviderCache.Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(ServiceProvider);
		}
		public IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		   Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument, TDataModel>()
			where TDataModel : class
		{
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
		NamedServiceResolver<IEntityPropertyModifier> EntityModifierValueConverterResolver { get; }
		IEntityModifierBuilder EntityModifierBuilder { get; }
		public DataSetAutoEntityProviderCache(
			IMetadataCollection Metas,
			IDataModelTypeCollection DataModelTypeCollection,
			IEnumerable<IEntityPropertyQueryConverterProvider> PropertyQueryConverterProviders,
			IEntityModifierBuilder EntityModifierBuilder
			)
		{
			this.Metas = Metas;
			this.DataModelTypeCollection = DataModelTypeCollection;
			this.PropertyQueryConverterProviders = PropertyQueryConverterProviders.OrderBy(p=>p.Property).ToArray();
			this.EntityModifierValueConverterResolver = EntityModifierValueConverterResolver;
			this.EntityModifierBuilder = EntityModifierBuilder;
		}

		void ValidateEntityTypes(Type TEntityDetail, IEntityType entity, params Type[] types)
		{
			foreach (var type in types)
			{
				var ne = Metas.EntityTypesByType.Get(type);
				if (ne == null)
					throw new ArgumentException($"自动化实体类型库中找不到类型{type}对应的实体");

				if (ne != entity)
					throw new ArgumentException($"类型{type}对应的实体{ne.FullName}和类型{TEntityDetail}对应的实体{entity.FullName}不一致");
			}
		}

		static IDataSetAutoEntityProviderSetting
			NewSetting<TKey, TEntityDetail, TEntityDetailTemp, TEntitySummary, TEntitySummaryTemp, TEntityEditable, TEntityEditableTemp, TQueryArgument, TDataModel>(
			QueryResultBuildHelper DetailQueryResultBuildHelper,
			QueryResultBuildHelper SummaryQueryResultBuildHelper,
			QueryResultBuildHelper EditableQueryResultBuildHelper,
			IEntityModifierBuilder EntityModifierBuilder
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
				EntityModifierBuilder.GetEntityModifier<TEntityEditable, TDataModel>(DataActionType.Delete)
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
			var dataType = DataModelType?? DataModelTypeCollection.Get(entity.FullName);

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
						EntityModifierBuilder
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
