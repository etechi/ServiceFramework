using SF.Core.ServiceManagement;
using SF.Data;
using SF.Data.EF6;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SF.Data
{
	public class IndexAttributeConvention : 
		AttributeToColumnAnnotationConvention<IndexAttribute, IndexAnnotation>
	{
		/// <summary>
		/// Constructs a new instance of the convention.
		/// </summary>
		static Lazy<Func<PropertyInfo, IEnumerable<System.ComponentModel.DataAnnotations.Schema.IndexAttribute>, IndexAnnotation>> IndexAnnotationCreator { get; } = new Lazy<Func<PropertyInfo, IEnumerable<System.ComponentModel.DataAnnotations.Schema.IndexAttribute>, IndexAnnotation>>(
			() =>
			{
				var p1 = Expression.Parameter(typeof(PropertyInfo));
				var p2 = Expression.Parameter(typeof(IEnumerable<System.ComponentModel.DataAnnotations.Schema.IndexAttribute>));
				var ctr = typeof(IndexAnnotation).GetConstructor(BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance, null, new[]
				{
					typeof(PropertyInfo),
					typeof(IEnumerable<System.ComponentModel.DataAnnotations.Schema.IndexAttribute>)
				}, null);
				return Expression.Lambda<Func<PropertyInfo, IEnumerable<System.ComponentModel.DataAnnotations.Schema.IndexAttribute>, IndexAnnotation>>(
					Expression.New(ctr, p1, p2),
					p1, p2
					).Compile();
			});

		public IndexAttributeConvention() : 
			base("Index", 
				(PropertyInfo p, IList<IndexAttribute> a) => IndexAnnotationCreator.Value(
					p, 
					from i in a orderby i.ToString() select 
					i.Name==null?
					new System.ComponentModel.DataAnnotations.Schema.IndexAttribute()
					{
						IsUnique = i.IsUnique
					}:
					new System.ComponentModel.DataAnnotations.Schema.IndexAttribute(i.Name, i.Order)
					{
						IsUnique=i.IsUnique
						
					})
				)
		{
		}
	}
	public class DbContext : System.Data.Entity.DbContext
	{
		public IServiceProvider ServiceProvider { get; }
		public DbContext(IServiceProvider ServiceProvider, string ConnectionString) :
			base(ConnectionString)
		{
			this.ServiceProvider = ServiceProvider;
		}
		public DbContext(IServiceProvider ServiceProvider,DbConnection Connection) :
			base(Connection,false)
		{
			this.ServiceProvider = ServiceProvider;
		}
		public DbContext(IServiceProvider ServiceProvider) :
			base(ServiceProvider.Resolve<DbConnection>(), false)
		{
			this.ServiceProvider = ServiceProvider;
		}
		interface IEntityDeclarer
		{
			void Declare(DbModelBuilder modelBuilder);
		}
		class EntityDeclarer<T> : IEntityDeclarer
			where T : class
		{
			public void Declare(DbModelBuilder modelBuilder)
			{
				modelBuilder.Entity<T>();
			}
		}
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			
			base.OnModelCreating(modelBuilder);
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

			modelBuilder.Conventions.Add<IndexAttributeConvention>();
			//System.Diagnostics.Debugger.Launch();
			var types = from ems in 
							ServiceProvider.Resolve<IEnumerable<IEntityDataModelSource>>().Select(ms=>ms.DataModels)
							.Concat(ServiceProvider.Resolve<IEnumerable<SF.Data.EntityDataModels>>())
						from et in ems.Types
						select et;
			foreach (var type in types.Distinct())
			{
				var declarer = (IEntityDeclarer)Activator.CreateInstance(typeof(EntityDeclarer<>).MakeGenericType(type));
				declarer.Declare(modelBuilder);
			}
		}

		string FormatEntityValidationException(DbEntityValidationException e)
		{
			return string.Join(";",
						from eve in e.EntityValidationErrors
						let type_name = eve.Entry.Entity.GetType().FullName
						from ve in eve.ValidationErrors
						select type_name + "." + ve.PropertyName + ":" + ve.ErrorMessage
					);
		}
		string FormatDbUpdateException(System.Data.Entity.Infrastructure.DbUpdateException e)
		{
			return (from ee in e.Entries
					let type = ee.Entity.GetType()
					let ctn = ee.Entity.ToString()
					select type.FullName + ":" + ctn
					).Join(";");
		}
		string FormatDbUpdateConcurrencyException(System.Data.Entity.Infrastructure.DbUpdateConcurrencyException e)
		{
			var ie = e.InnerException as System.Data.Entity.Core.OptimisticConcurrencyException;
			if (ie != null)
				return (from ee in ie.StateEntries
						let type = ee.Entity.GetType()
						let id = ee.EntityKey.EntityKeyValues.Select(kv => kv.Key + "=" + kv.Value).Join(",")
						let ctn = ee.Entity.ToString()
						select type.FullName + ":" + id + ":" + ctn
					).Join(";");

			return (from ee in e.Entries
					let type = ee.Entity.GetType()
					let ctn = ee.Entity.ToString()
					select type.FullName + ":" + ctn
					).Join(";");
		}
		Exception MapException(System.Exception e)
		{
			var ve = e as DbEntityValidationException;
			if (ve != null)
				return new DbValidateException(
					"数据实体验证失败：" + FormatEntityValidationException(ve),
					ve
					);
			var ce = e as System.Data.Entity.Infrastructure.DbUpdateConcurrencyException;
			if (ce != null)
				return new DbUpdateConcurrencyException(
					"数据并发更新错误：" + FormatDbUpdateConcurrencyException(ce),
					ce);

			var ue = e as System.Data.Entity.Infrastructure.DbUpdateException;
			if (ue != null)
			{
				if (ue.Entries.All(s => s.State == EntityState.Added))
					return new DbDuplicatedKeyException(
						"主键或约束冲突：" + e.GetRootException().Message + "：" + FormatDbUpdateException(ue),
						ue);
				else
					return new DbUpdateException(
						"数据更新失败：" + e.GetRootException().Message + "：" + FormatDbUpdateException(ue),
						ue);
			}


			return new DbException("数据操作失败：" + e.GetRootException().Message, e);
		}
		public override int SaveChanges()
		{
			try
			{
				return base.SaveChanges();
			}
			catch (Exception e)
			{
				throw MapException(e);
			}
		}
		public override async Task<int> SaveChangesAsync()
		{
			try
			{
				return await base.SaveChangesAsync();
			}
			catch (Exception e)
			{
				throw MapException(e);
			}
		}
	}
	

}
