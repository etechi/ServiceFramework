using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ServiceProtocol.Data.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SF.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Data.Entity.EntityFrameworkCore
{
	class EntityItem
	{
		public string Prefix { get; set; }
		public Type Type { get; set; }
	}
	class DataModelCustomizer : ModelCustomizer
	{
		public override void Customize(ModelBuilder modelBuilder, DbContext dbContext)
		{
			var mi = dbContext.GetService<DataModalInitializer>();
			if (mi != null)
				mi.Init(modelBuilder);
			base.Customize(modelBuilder, dbContext);
		}
	}
	class DataModalInitializer
	{
		public EntityItem[] EntityItems { get; }
		public DataModalInitializer(EntityItem[] EntityItems)
		{
			
			this.EntityItems = EntityItems;
		}
		public void Init(ModelBuilder modelBuilder)
		{
			foreach(var item in EntityItems)
			{
				var entity = modelBuilder.Entity(item.Type);
				if (!string.IsNullOrWhiteSpace(item.Prefix))
				{
					var tableAttr = item.Type.GetCustomAttribute<TableAttribute>();
					var tableName = item.Prefix + (tableAttr?.Name ?? item.Type.Name);
					entity.ToTable(tableName);
				}
				
				var indexs = (
					from p in item.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.FlattenHierarchy)
					from idx in p.GetCustomAttributes().Where(a => a is IndexAttribute).Cast<IndexAttribute>()
					let rec = new { name = idx.Name ?? p.Name, field=p.Name, unique = idx.IsUnique, clustered = idx.IsClustered, order = idx.Order }
					group rec by rec.name into g
					select new {
						name = g.Key,
						unique =g.Any(i=>i.unique),
						clustered =g.Any(i=>i.clustered),
						fields =g.OrderBy(i => i.order).Select(i=>i.field).ToArray() }
					).ToArray();
				foreach(var idx in indexs)
				{
					entity.HasIndex(idx.fields).IsUnique(idx.unique);
				}
			}
		}
	}
	class DataModalLoaderExtension : IDbContextOptionsExtension
	{
		public EntityItem[] EntityItems { get; }
		public DataModalLoaderExtension(EntityItem[] EntityItems)
		{
			this.EntityItems = EntityItems;
		}
		public void ApplyServices(IServiceCollection services)
		{
			services.AddSingleton(new DataModalInitializer(EntityItems));
		}
	}
	public static class DbContextOptionBuilderExtension
	{
		public static DbContextOptionsBuilder LoadDataModels(this DbContextOptionsBuilder Builder,IServiceProvider sp)
		{
			Builder.ReplaceService<IModelCustomizer, DataModelCustomizer>();

			((IDbContextOptionsBuilderInfrastructure)Builder).AddOrUpdateExtension(
				new DataModalLoaderExtension(
					(
					from ems in sp.GetService<IEnumerable<SF.Data.Entity.EntityModels>>()
					from et in ems.Types
					select new EntityItem { Prefix = ems.Prefix, Type = et }
					).ToArray()
					)
				);
			return Builder;
		}


	}

}
