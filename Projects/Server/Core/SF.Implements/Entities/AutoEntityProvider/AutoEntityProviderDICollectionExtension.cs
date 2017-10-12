﻿using System;
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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Entities.AutoEntityProvider.Internals.ValueTypes;
using System.ComponentModel;
using SF.Entities.AutoEntityProvider.Internals.PropertyModifiers;
using SF.Entities.AutoEntityProvider.Internals.EntityModifiers;
using SF.Entities.AutoEntityProvider.Internals.PropertyQueryConveters;
using SF.Entities.AutoEntityProvider.Internals.DataModelBuilders;
using SF.Entities.AutoEntityProvider.Internals.DataModelBuilders.AttributeGenerators;
using SF.Entities.AutoEntityProvider.Internals.DataModelBuilders.TypeMappers;
using SF.Entities.AutoEntityProvider.Internals.DataModelBuilders.Providers;
using SF.Entities.AutoEntityProvider.Internals.QueryFilterProviders;

namespace SF.Core.ServiceManagement
{
	
	public static class AutoEntityProviderDICollectionExtension
	{
		class DataModuleSource : SF.Data.IEntityDataModelSource
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
			sc.AddAttributeGenerator<NoneAttributeGenerator,CommentAttribute> ();
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


			sc.AddTransient<DataModelBuilder>();

			sc.AddTransient<SystemTypeMetadataBuilder>();
			


			sc.AddSingleton<DataSetAutoEntityProviderFactory>();

			sc.AddSingleton<IMetadataCollection>(sp =>sp.Resolve<SystemTypeMetadataBuilder>().Build());


			//Entity Modify Support
			sc.AddSingleton<IEntityPropertyModifierProvider, AutoKeyPropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, ServiceScopeIdPropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, DefaultPropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, JsonDataPropertyModifierProvider>();
			sc.AddSingleton<IEntityModifierProvider, PropertyEntityModifierProvider>();
			sc.AddSingleton<IEntityModifierBuilder, EntityModifierBuilder>();

			//Entity Query Support
			sc.AddSingleton<IEntityPropertyQueryConverterProvider, JsonDataQueryConverterProvider>();


			sc.AddSingleton<IDataSetAutoEntityProviderCache, DataSetAutoEntityProviderCache>();
			sc.AddScoped<IDataSetAutoEntityProviderFactory, DataSetAutoEntityProviderFactory>();

			sc.AddSingleton(
				sp => sp.Resolve<DataModelBuilder>().Build(string.Empty)
				);

			sc.AddSingleton<SF.Data.IEntityDataModelSource>(
				sp =>
					new DataModuleSource
					{
						DataModels = new EntityDataModels(
							sp.Resolve<IDataModelTypeCollection>().Values.ToArray(),
							Prefix
							)
					}
				);

			return sc;
		}
	}
}
