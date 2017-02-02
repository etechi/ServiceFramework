using SF.Core.DI;
using SF.Data.Entity;
using SF.Data.Entity.EF6;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
namespace SF.Data.Entity
{
	public class IndexAttributeConvention : 
		AttributeToColumnAnnotationConvention<IndexAttribute, IndexAnnotation>
	{
		/// <summary>
		/// Constructs a new instance of the convention.
		/// </summary>
		public IndexAttributeConvention() : 
			base("Index", 
				(PropertyInfo p, IList<IndexAttribute> a) =>(IndexAnnotation) Activator.CreateInstance(
					typeof(IndexAnnotation),
					p, 
					from i in a orderby i.ToString() select 
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
		public DbContext(IServiceProvider ServiceProvider,string ConnectionString):
			base(ConnectionString)
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
			modelBuilder.Conventions.Add<IndexAttributeConvention>();
			System.Diagnostics.Debugger.Launch();
			var types = from ems in ServiceProvider.Resolve<IEnumerable<SF.Data.Entity.EntityModels>>()
						from et in ems.Types
						select et;
			foreach (var type in types.Distinct())
			{
				var declarer = (IEntityDeclarer)Activator.CreateInstance(typeof(EntityDeclarer<>).MakeGenericType(type));
				declarer.Declare(modelBuilder);
			}
		}
	}
	

}
