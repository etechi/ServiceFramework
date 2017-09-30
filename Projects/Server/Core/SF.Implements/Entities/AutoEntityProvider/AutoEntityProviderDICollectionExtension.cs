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
		   where TQueryArgument :class
		{
			var f = sp.Resolve<DataSetAutoEntityProviderFactory>();
			return f.Create<TKey,TEntityDetail,TEntitySummary,TEntityEditable,TQueryArgument>(sp);
		}

		static void AddPrimitiveValueType<T>(this IServiceCollection sc)
		{
			sc.AddSingleton<IValueType, PrimitiveValueType<T>>();
		}
		static void AddAttributeGenerator<G,A>(this IServiceCollection sc)
			where G:IDataModelAttributeGenerator
		{
			sc.AddSingleton<IDataModelAttributeGenerator, G>(typeof(A).FullName);
		}
		public static IServiceCollection AddAutoEntityService(this IServiceCollection sc,string Prefix=null)
		{
			sc.AddPrimitiveValueType<char>();
			sc.AddPrimitiveValueType<byte>();
			sc.AddPrimitiveValueType<sbyte>();
			sc.AddPrimitiveValueType<bool>();
			sc.AddPrimitiveValueType<short>();
			sc.AddPrimitiveValueType<ushort>();
			sc.AddPrimitiveValueType<int>();
			sc.AddPrimitiveValueType<uint>();
			sc.AddPrimitiveValueType<long>();
			sc.AddPrimitiveValueType<ulong>();
			sc.AddPrimitiveValueType<float>();
			sc.AddPrimitiveValueType<double>();
			sc.AddPrimitiveValueType<decimal>();
			sc.AddPrimitiveValueType<DateTime>();
			sc.AddPrimitiveValueType<string>();


			sc.AddAttributeGenerator<KeyAttributeGenerator,KeyAttribute>();
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

			sc.AddTransient<SystemTypeMetadataBuilder>();
			sc.AddTransient<DataModelTypeBuilder>();
			sc.AddSingleton<DataSetAutoEntityProviderFactory>();
			sc.AddSingleton<IValueTypeResolver, ValueTypeResolver>();

			sc.AddSingleton<IMetadataCollection>(sp =>
				sp.Resolve<SystemTypeMetadataBuilder>().Build()
				);
			
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
