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

using System.Linq;
using SF.Sys.Entities.AutoEntityProvider;
using SF.Sys.Entities.AutoEntityProvider.Internals;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Sys.Entities.AutoEntityProvider.Internals.ValueTypes;
using System.ComponentModel;
using SF.Sys.Entities.AutoEntityProvider.Internals.PropertyModifiers;
using SF.Sys.Entities.AutoEntityProvider.Internals.EntityModifiers;
using SF.Sys.Entities.AutoEntityProvider.Internals.PropertyQueryConveters;
using SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders;
using SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders.AttributeGenerators;
using SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders.TypeMappers;
using SF.Sys.Entities.AutoEntityProvider.Internals.DataModelBuilders.Providers;
using SF.Sys.Entities.AutoEntityProvider.Internals.QueryFilterProviders;
using SF.Sys.Data;
using SF.Sys.Annotations;
using SF.Sys.Entities;

namespace SF.Sys.Services
{

	public static class AutoEntityProviderDICollectionExtension
	{
		class DataModuleSource : IEntityDataModelSource
		{
			 public EntityDataModels DataModels { get; set; }
		}
	

		static void AddAttributeGenerator<G,A>(this IServiceCollection sc)
			where G:IDataModelAttributeGenerator
		{
			sc.AddSingleton<IDataModelAttributeGenerator, G>(typeof(A).FullName);
		}
		public static IServiceCollection AddAutoEntityService(this IServiceCollection sc,string Prefix=null)
		{

			//Value Type Supports
			sc.AddSingleton<IValueTypeProvider, PrimitiveValueTypeProvider>();
			sc.AddSingleton<IValueTypeProvider, JsonDataValueTypeProvider>();
			sc.AddSingleton<IValueTypeResolver, ValueTypeResolver>();



			//Data Model Attribute Generator Supports
			sc.AddAttributeGenerator<KeyAttributeGenerator,KeyAttribute>();
			sc.AddAttributeGenerator<JsonDataAttributeGenerator, JsonDataAttribute>();
			sc.AddAttributeGenerator<ColumnAttributeGenerator,ColumnAttribute> ();
			sc.AddAttributeGenerator<NoneAttributeGenerator,EntityObjectAttribute> ();
			sc.AddAttributeGenerator<NoneAttributeGenerator,EntityIdentAttribute> ();
			sc.AddAttributeGenerator<StringLengthAttributeGenerator,StringLengthAttribute> ();
			sc.AddAttributeGenerator<RequiredAttributeGenerator,RequiredAttribute> ();
			sc.AddAttributeGenerator<MaxLengthAttributeGenerator,MaxLengthAttribute> ();
			sc.AddAttributeGenerator<MinLengthAttributeGenerator,MinLengthAttribute> ();
			sc.AddAttributeGenerator<DatabaseGeneratedAttributeGenerator,DatabaseGeneratedAttribute> ();
			//sc.AddAttributeGenerator<NoneAttributeGenerator,CommentAttribute> ();
			sc.AddAttributeGenerator<NoneAttributeGenerator,ReadOnlyAttribute> ();
			sc.AddAttributeGenerator<NoneAttributeGenerator,TableVisibleAttribute> ();
			sc.AddAttributeGenerator<NoneAttributeGenerator,HiddenAttribute> ();
			sc.AddAttributeGenerator<NoneAttributeGenerator,TableRowsAttribute> ();
			sc.AddAttributeGenerator<NoneAttributeGenerator,EntityTitleAttribute> ();
			sc.AddAttributeGenerator<IndexAttributeGenerator,IndexAttribute> ();
			sc.AddAttributeGenerator<TableAttributeGenerator,TableAttribute> ();



			//Data Model Type Mapper
			sc.AddSingleton<IDataModelTypeMapper, JsonDataTypeMapper>();

			//DataModel Build Providers
			sc.AddSingleton<IDataModelBuildProvider, DefaultDataModelBuildProvider>();
			sc.AddSingleton<IDataModelBuildProvider, DataModelRelationBuildProvider>();

			sc.AddSingleton<IDataModelTypeBuildProvider, DefaultDataModelTypeBuildProvider>();
			sc.AddSingleton<IDataModelTypeBuildProvider, DataModelAutoPrimaryKeyProvider>();
			sc.AddSingleton<IDataModelTypeBuildProvider, DataModelTableNameProvider>();
			sc.AddSingleton<IDataModelTypeBuildProvider, DataModelServiceScopeProvider>();



			sc.AddSingleton<IDataModelPropertyBuildProvider, DefaultDataModelValuePropertyBuildProvider>();
			sc.AddSingleton<IDataModelPropertyBuildProvider, DataModelEntityIdentPropertyBuildProvider>();


			sc.AddSingleton<IQueryFilterProvider, PropQueryFilterProvider>();
			sc.AddSingleton<IQueryFilterProvider, ServiceScopeQueryFilterProvider>();

			sc.AddSingleton<IPropertyQueryFilterProvider, DateTimeRangeToRangePropQueryFilterProvider>();
			sc.AddSingleton<IPropertyQueryFilterProvider, EntityLogicStatePropQueryFilterProvider>();
			sc.AddSingleton<IPropertyQueryFilterProvider, NullablePropQueryFilterProvider>();
			sc.AddSingleton<IPropertyQueryFilterProvider, NullableRangePropQueryFilterProvider>();
			sc.AddSingleton<IPropertyQueryFilterProvider, NullableRangeToRangePropQueryFilterProvider>();
			sc.AddSingleton<IPropertyQueryFilterProvider, ObjectKeyPropQueryFilterProvider>();
			sc.AddSingleton<IPropertyQueryFilterProvider, OptionPropQueryFilterProvider>();
			sc.AddSingleton<IPropertyQueryFilterProvider, QueryBooleanPropQueryFilterProvider>(); 
			sc.AddSingleton<IPropertyQueryFilterProvider, RangePropQueryFilterProvider>();
			sc.AddSingleton<IPropertyQueryFilterProvider, RangeToRangePropQueryFilterProvider>();
			sc.AddSingleton<IPropertyQueryFilterProvider, StringPropQueryFilterProvider>();

			sc.AddSingleton<IQueryFilterCache, QueryFilterCache>();


			sc.AddSingleton<IQueryResultBuildHelperCache, QueryResultBuildHelperCache>();

			sc.AddTransient<DataModelBuilder>();

			sc.AddTransient<SystemTypeMetadataBuilder>();
			


			
			sc.AddSingleton<IMetadataCollection>(sp =>sp.Resolve<SystemTypeMetadataBuilder>().Build());


			//Entity Modify Support
			sc.AddSingleton<IEntityPropertyModifierProvider, AutoKeyPropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, CreatedTimePropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, UpdatedTimePropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, ReadOnlyPropertyModifierProvider>();

			sc.AddSingleton<IEntityPropertyModifierProvider, SkipWhenDefaultPropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, SkipWhenKeyModifyPropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, ServiceScopeIdPropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, DefaultPropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, JsonDataPropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, MultipleRelationPropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, MultipleRelationIdentPropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, SingleRelationPropertyModifierProvider>();
			sc.AddSingleton<IEntityModifierProvider, PropertyEntityModifierProvider>();
			sc.AddSingleton<IEntityModifierCache, EntityModifierCache>();

			//Entity Query Support

			sc.AddSingleton<IEntityPropertyQueryConverterProvider, JsonDataQueryConverterProvider>();
			sc.AddSingleton<IEntityPropertyQueryConverterProvider, EntityNameQueryConverterProvider>();
			sc.AddSingleton<IEntityPropertyQueryConverterProvider, DefaultQueryConverterProvider>();
			sc.AddSingleton<IEntityPropertyQueryConverterProvider, SingleRelationQueryConverterProvider>();
			sc.AddSingleton<IEntityPropertyQueryConverterProvider, MultipleRelationQueryConverterProvider>();
			sc.AddSingleton<IEntityPropertyQueryConverterProvider, MultipleRelationIdentQueryConverterProvider>();
			sc.AddSingleton<IEntityPropertyQueryConverterProvider, SkipWhenDefaultQueryConverterProvider>();

			//暂不支持自动生成数据模型
			//sc.AddSingleton<IDataSetAutoEntityProviderCache, DataSetAutoEntityProviderCache>();
			//sc.AddTransient<IDataSetAutoEntityProviderFactory, DataSetAutoEntityProviderFactory>();

			sc.AddSingleton(
				sp => sp.Resolve<DataModelBuilder>().Build(string.Empty)
				);

			sc.AddSingleton<SF.Sys.Data.IEntityDataModelSource>(
				sp =>
					new DataModuleSource
					{
						DataModels = new EntityDataModels(
							sp.Resolve<IDataModelTypeCollection>().Values.ToArray(),
							Prefix
							)
					}
				);

			sc.AddSingleton<IPagingQueryBuilderCache, PagingQueryBuilderCache>();

			return sc;
		}
	}
}
