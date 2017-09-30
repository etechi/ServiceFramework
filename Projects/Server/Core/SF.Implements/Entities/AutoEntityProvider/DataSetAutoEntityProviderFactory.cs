using SF.Core.ServiceManagement;
using SF.Entities.AutoEntityProvider.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SF.Entities.AutoEntityProvider
{
	public class DataSetAutoEntityProviderFactory
	{
		System.Collections.Concurrent.ConcurrentDictionary<Type, Lazy<IDataSetAutoEntityProviderSetting>> Creators =
			new System.Collections.Concurrent.ConcurrentDictionary<Type, Lazy<IDataSetAutoEntityProviderSetting>>();
		IMetadataCollection Metas { get; }
		IDataModelTypeCollection DataModelTypeCollection { get; }
		NamedServiceResolver<IValueConverter> ValueConverterResolver { get; }
		NamedServiceResolver<IEntityPropertyModifier> EntityModifierValueConverterResolver { get; }
		public DataSetAutoEntityProviderFactory(
			IMetadataCollection Metas,
			IDataModelTypeCollection DataModelTypeCollection,
			NamedServiceResolver<IValueConverter> ValueConverterResolver,
			NamedServiceResolver<IEntityPropertyModifier> EntityModifierValueConverterResolver
			)
		{
			this.Metas = Metas;
			this.DataModelTypeCollection = DataModelTypeCollection;
			this.ValueConverterResolver = ValueConverterResolver;
			this.EntityModifierValueConverterResolver = EntityModifierValueConverterResolver;
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
			EntityModifier InitModifier,
			EntityModifier UpdateModifier
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
				(IEntityModifier<TEntityEditable, TDataModel>)InitModifier,
				(IEntityModifier<TEntityEditable, TDataModel>)UpdateModifier
				);
			}
		public IDataSetAutoEntityProvider<TKey,TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>  
			Create<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(IServiceProvider sp)
		{
			var tDetail = typeof(TEntityDetail);
			if (!Creators.TryGetValue(tDetail, out var setting))
			{
				setting = new Lazy<IDataSetAutoEntityProviderSetting>(() =>
				  {
					  var tKey = typeof(TKey);
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
					  var dataType = DataModelTypeCollection.Get(entity.FullName);

					  var detailHelper = new QueryResultBuildHelperCreator(dataType, tDetail, ValueConverterResolver).Build();
					  var summaryHelper = new QueryResultBuildHelperCreator(dataType, tSummary, ValueConverterResolver).Build();
					  var editableHelper = new QueryResultBuildHelperCreator(dataType, tEditable, ValueConverterResolver).Build();
					  var initModifier = new EntityModifierCreator(dataType, tEditable, EntityModifierValueConverterResolver, "Init").Build();
					  var updateModifier = new EntityModifierCreator(dataType, tEditable, EntityModifierValueConverterResolver, "Update").Build();

					  var newSetting = typeof(DataSetAutoEntityProviderFactory).GetMethods(
						  "NewSetting",
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
							initModifier,
							updateModifier
						  }
						 );
				  });
				
				setting = Creators.GetOrAdd(tDetail,setting);
			}
			return (IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>)setting.Value.FuncCreateProvider.Value(sp);
		}
	}


}
