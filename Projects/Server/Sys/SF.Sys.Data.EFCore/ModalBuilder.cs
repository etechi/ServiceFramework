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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Reflection;

namespace SF.Sys.Data.EntityFrameworkCore
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

			foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
			{
				relationship.DeleteBehavior = DeleteBehavior.Restrict;
			}
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

			//System.Diagnostics.Debugger.Launch();
			Builder.ReplaceService<IModelCustomizer, DataModelCustomizer>();
			var models = sp.GetService<IEnumerable<IEntityDataModelSource>>().Select(ms => ms.DataModels)
							.Concat(sp.GetService<IEnumerable<SF.Sys.Data.EntityDataModels>>())
							.ToArray();
			((IDbContextOptionsBuilderInfrastructure)Builder).AddOrUpdateExtension(
				new DataModalLoaderExtension(
					(
					from ems in models
					from et in ems.Types
					select new EntityItem { Prefix = ems.Prefix, Type = et }
					).ToArray()
					)
				);
			return Builder;
		}


	}

}
