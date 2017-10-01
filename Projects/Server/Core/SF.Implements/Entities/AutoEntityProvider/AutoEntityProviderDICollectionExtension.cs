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
using SF.Entities.AutoEntityProvider.Internals.DataModelAttributeGenerators;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Entities.AutoEntityProvider.Internals.ValueTypes;
using System.ComponentModel;
using SF.Entities.AutoEntityProvider.Internals.PropertyModifiers;
using SF.Entities.AutoEntityProvider.Internals.EntityModifiers;
using SF.Entities.AutoEntityProvider.Internals.DataModelTypeMappers;
using SF.Entities.AutoEntityProvider.Internals.PropertyQueryConveters;

namespace SF.Core.ServiceManagement
{
	
	public static class AutoEntityProviderDICollectionExtension
	{
		class DataModuleSource : SF.Data.IEntityDataModelSource
		{
			 public EntityDataModels DataModels { get; set; }
		}
		static IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument> 
			CreateEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>(
			IServiceProvider sp)
		{
			var f = sp.Resolve<DataSetAutoEntityProviderFactory>();
			return f.Create<TKey,TEntityDetail,TEntitySummary,TEntityEditable,TQueryArgument>(sp);
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

			sc.AddTransient<SystemTypeMetadataBuilder>();
			sc.AddTransient<DataModelTypeBuilder>();
			sc.AddSingleton<DataSetAutoEntityProviderFactory>();

			sc.AddSingleton<IMetadataCollection>(sp =>sp.Resolve<SystemTypeMetadataBuilder>().Build());


			//Entity Modify Support
			sc.AddSingleton<IEntityPropertyModifierProvider, AutoKeyPropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, DefaultPropertyModifierProvider>();
			sc.AddSingleton<IEntityPropertyModifierProvider, JsonDataPropertyModifierProvider>();
			sc.AddSingleton<IEntityModifierProvider, PropertyEntityModifierProvider>();

			//Entity Query Support
			sc.AddSingleton<IEntityPropertyQueryConverterProvider, JsonDataQueryConverterProvider>();


			sc.AddTransient(
				typeof(IDataSetAutoEntityProvider<, , , , >), 
				typeof(AutoEntityProviderDICollectionExtension).GetMethodExt(
					nameof(CreateEntityProvider),
					BindingFlags.Static | BindingFlags.NonPublic |  BindingFlags.InvokeMethod,
					typeof(IServiceProvider))
				);
			sc.AddSingleton(
				sp => sp.Resolve<DataModelTypeBuilder>().Build(string.Empty)
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
