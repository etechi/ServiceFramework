using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SF.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace SF.Data.EntityFrameworkCore
{
	class EntityItem
	{
		public string Prefix { get; set; }
		public Type Type { get; set; }
	}
	class DataModelCustomizer : ModelCustomizer
	{
#if NETSTANDARD2_0
		public DataModelCustomizer( ModelCustomizerDependencies dependencies) : base(dependencies)
		{
		}
#endif
		public override void Customize(ModelBuilder modelBuilder, DbContext dbContext)
		{
			base.Customize(modelBuilder, dbContext);
			var mi = dbContext.GetService<DataModalInitializer>();
			if (mi != null)
				mi.Init(modelBuilder);
		}
	}
	class DataModalInitializer
	{
		public EntityItem[] EntityItems { get; }
		public DataModalInitializer(EntityItem[] EntityItems)
		{
			
			this.EntityItems = EntityItems;
		}
		void BuildIndex(EntityItem item,EntityTypeBuilder builder)
		{
			var indexs = (
					from p in item.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance  | BindingFlags.FlattenHierarchy)
					from idx in p.GetCustomAttributes().Where(a => a is IndexAttribute).Cast<IndexAttribute>()
					let rec = new {
						name = idx.Name ?? p.Name,
						field = p.Name,
						unique = idx.IsUnique,
						clustered = idx.IsClustered,
						order = idx.Order,
						level =p.DeclaringType.GetInheritLevel()
					}
					group rec by rec.name into g
					select new
					{
						name = g.Key,
						unique = g.Any(i => i.unique),
						clustered = g.Any(i => i.clustered),
						fields = g.GroupBy(a=>a.field)
								.Select(gi=>gi.OrderByDescending(i=>i.level).First())
								.OrderBy(i => i.order)
								.Select(i => i.field)
								.ToArray()
					}
					).ToArray();
			foreach (var idx in indexs)
			{
				builder.HasIndex(idx.fields).IsUnique(idx.unique);
			}
		}
		void TryBuildCompositPrimaryKey(EntityItem item, EntityTypeBuilder builder)
		{
			var keys = (from p in item.Type
				.GetProperties(
					BindingFlags.Public | BindingFlags.Instance| BindingFlags.FlattenHierarchy
					)
						where p.GetCustomAttribute<KeyAttribute>(true) != null
						let col = p.GetCustomAttribute<ColumnAttribute>(true)
						orderby col?.Order
						select col?.Name ?? p.Name
					).ToArray();
			if (keys.Length <= 1)
				return;
			foreach (var key in builder.Metadata.GetKeys().ToArray())
				builder.Metadata.RemoveKey(key.Properties);
			builder.HasKey(keys);
		}
		public void Init(ModelBuilder modelBuilder)
		{
			foreach (var item in EntityItems)
			{
				var builder = modelBuilder.Entity(item.Type);
				if (!string.IsNullOrWhiteSpace(item.Prefix))
				{
					var tableAttr = item.Type.GetCustomAttribute<TableAttribute>();
					var tableName = item.Prefix + (tableAttr?.Name ?? item.Type.Name);
					builder.ToTable(tableName);
				}

				TryBuildCompositPrimaryKey(item, builder);
				BuildIndex(item, builder);
			}
			
		}
	}
	class DataModalLoaderExtension : IDbContextOptionsExtension
	{
		public EntityItem[] EntityItems { get; }

		public string LogFragment => "SF.EFCore";

		public DataModalLoaderExtension(EntityItem[] EntityItems)
		{
			this.EntityItems = EntityItems;
		}
		public void ApplyServices(IServiceCollection services)
		{
			services.AddSingleton(new DataModalInitializer(EntityItems));
		}
#if NETSTANDARD2_0
		bool IDbContextOptionsExtension.ApplyServices(IServiceCollection services)
		{
			services.AddSingleton(new DataModalInitializer(EntityItems));
			return true;
		}

		public long GetServiceProviderHashCode()
		{
			return 0;
		}

		public void Validate(IDbContextOptions options)
		{
			//throw new NotImplementedException();
		}
#endif
	}
	public static class DbContextOptionBuilderExtension
	{
		public static DbContextOptionsBuilder LoadDataModels(this DbContextOptionsBuilder Builder,IServiceProvider sp)
		{
			Builder.ReplaceService<IModelCustomizer, DataModelCustomizer>();

			((IDbContextOptionsBuilderInfrastructure)Builder).AddOrUpdateExtension(
				new DataModalLoaderExtension(
					(
					from ems in sp.GetService<IEnumerable<SF.Data.EntityDataModels>>()
					from et in ems.Types
					select new EntityItem { Prefix = ems.Prefix, Type = et }
					).ToArray()
					)
				);
			return Builder;
		}


	}

}
