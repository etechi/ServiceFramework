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
		   where TQueryArgument : IQueryArgument<TKey>
		{
			var f = sp.Resolve<DataSetAutoEntityProviderFactory>();
			return f.Create<TKey,TEntityDetail,TEntitySummary,TEntityEditable,TQueryArgument>(sp);
		}

		static void AddPrimitiveValueType<T>(this IServiceCollection sc)
		{
			sc.AddSingleton<IValueType, PrimitiveValueType<T>>();
		}
		public static IServiceCollection AddAutoEntityService(this IServiceCollection sc)
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


			sc.AddSingleton<IDataModelAttributeGenerator, KeyAttributeGenerator>(typeof(KeyAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, ColumnAttributeGenerator>(typeof(ColumnAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, NoneAttributeGenerator>(typeof(EntityObjectAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, NoneAttributeGenerator>(typeof(EntityIdentAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, StringLengthAttributeGenerator>(typeof(StringLengthAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, RequiredAttributeGenerator>(typeof(RequiredAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, MaxLengthAttributeGenerator>(typeof(MaxLengthAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, MinLengthAttributeGenerator>(typeof(MinLengthAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, DatabaseGeneratedAttributeGenerator>(typeof(DatabaseGeneratedAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, NoneAttributeGenerator>(typeof(CommentAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, NoneAttributeGenerator>(typeof(ReadOnlyAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, NoneAttributeGenerator>(typeof(TableVisibleAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, NoneAttributeGenerator>(typeof(HiddenAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, NoneAttributeGenerator>(typeof(TableRowsAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, NoneAttributeGenerator>(typeof(EntityTitleAttribute).FullName);
			sc.AddSingleton<IDataModelAttributeGenerator, IndexAttributeGenerator>(typeof(IndexAttribute).FullName);

			sc.AddTransient<SystemTypeMetadataBuilder>();
			sc.AddTransient<DataModelTypeBuilder>();
			sc.AddSingleton<DataSetAutoEntityProviderFactory>();
			sc.AddSingleton<IValueTypeResolver, ValueTypeResolver>();

			sc.AddSingleton<IMetadataCollection>(sp =>
				sp.Resolve<SystemTypeMetadataBuilder>().Build()
				);
			
			sc.AddTransient(
				typeof(IDataSetAutoEntityProvider<, , , , >), 
				typeof(ServiceProviderBuilder).GetMethodExt(nameof(CreateEntityProvider), typeof(IServiceProvider))
				);

			sc.AddSingleton<SF.Data.IEntityDataModelSource>(
				sp =>
					new DataModuleSource
					{
						DataModels = new EntityDataModels(
							sp.Resolve<DataModelTypeBuilder>().Build(),
							string.Empty
							)
					}
				);

			return sc;
		}
	}
}
