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

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using SF.Sys.Data;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Internal;
using Remotion.Linq.Parsing.Structure;
using Microsoft.EntityFrameworkCore.Query;

namespace SF.Sys.Data.EntityFrameworkCore
{

	//public class ServiceProtocolDataIndexAttributeConvention : AttributeToColumnAnnotationConvention<ServiceProtocol.Entities.IndexAttribute, IndexAnnotation>
	//{
	//	public ServiceProtocolDataIndexAttributeConvention() :
	//		base("Index",
	//			(p, a) => new IndexAnnotation(
	//				a.Select(i =>
	//					i.Name==null?
	//					new System.ComponentModel.DataAnnotations.Schema.IndexAttribute
	//					{
	//						IsClustered = i.IsClustered,
	//						IsUnique = i.IsUnique,
	//						Order = i.Order
	//					}:new System.ComponentModel.DataAnnotations.Schema.IndexAttribute(i.Name)
	//					  {
	//						  IsClustered=i.IsClustered,
	//						  IsUnique=i.IsUnique,
	//						  Order=i.Order
	//					  }
	//					)
	//			)
	//			)
	//	{ }
	//}

	public class EntityDbContextProvider<T> :
		EntityDbContextProvider
		where T : DbContext
	{
		public EntityDbContextProvider(T Context, IDataContext DataContext) : base(Context, DataContext)
		{
		}
	}
	public class EntityDbContextProvider :
        IDataContextProvider,
        IDataContextExtension
    {
		public DbContext DbContext { get; }
		public IDataContext DataContext { get; }
		public bool IsChanged { get; private set; }
		internal void SetChanged()
		{
			
			IsChanged = true;
		}
		public EntityDbContextProvider(DbContext DbContext, IDataContext DataContext)
		{
			this.DbContext = DbContext;
			var cs = DbContext as IDbDataContextState;
			if (cs != null)
				System.Diagnostics.Debugger.Log(1, "DB", "DBContext被分配:"+cs.State + "\n");
			DbContext.ChangeTracker.AutoDetectChangesEnabled = false;
			this.DataContext = DataContext;
		}
		public IAsyncQueryableProvider AsyncQueryableProvider
		{
			get
			{
				return EntityFrameworkCore.AsyncQueryableProvider.Instance;
			}
		}

		public IEntityQueryableProvider EntityQueryableProvider
		{
			get
			{
				return EntityFrameworkCore.AsyncQueryableProvider.Instance;
			}
		}
		DbTransaction _tran;
		public DbTransaction Transaction {
			get => _tran;
			set => 
				DbContext.Database.UseTransaction(_tran=value);
		}



		//protected override void OnModelCreating(DbModelBuilder modelBuilder)
		//{
		//	base.OnModelCreating(modelBuilder);
		//	modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
		//	modelBuilder.Conventions.Add(new ServiceProtocolDataIndexAttributeConvention());
		//}
		//string FormatEntityValidationException(Microsoft.EntityFrameworkCore ValidationException e)
		//{
		//	return string.Join(";",
		//				from eve in e.EntityValidationErrors
		//				let type_name = eve.Entry.Entity.GetType().FullName
		//				from ve in eve.ValidationErrors
		//				select type_name + "." + ve.PropertyName + ":" + ve.ErrorMessage
		//			);
		//}
		//string FormatDbUpdateException(System.Data.Entity.Infrastructure.DbUpdateException e)
		//{
		//	return (from ee in e.Entries
		//			let type = ee.Entity.GetType()
		//			let ctn = ee.Entity.ToString()
		//			select type.FullName + ":" + ctn
		//			).Join(";");
		//}
		//string FormatDbUpdateConcurrencyException(System.Data.Entity.Infrastructure.DbUpdateConcurrencyException e)
		//{
		//	var ie = e.InnerException as System.Data.Entity.Core.OptimisticConcurrencyException;
		//	if(ie!=null)
		//		return (from ee in ie.StateEntries
		//				let type = ee.Entity.GetType()
		//				let id=ee.EntityKey.EntityKeyValues.Select(kv=>kv.Key+"="+kv.Value).Join(",")
		//				let ctn = ee.Entity.ToString()
		//				select type.FullName + ":" +id+":"+ ctn
		//			).Join(";");

		//	return (from ee in e.Entries
		//			let type = ee.Entity.GetType()
		//			let ctn = ee.Entity.ToString()
		//			select type.FullName + ":" + ctn
		//			).Join(";");
		//}
		Exception MapException(System.Exception e)
		{
			//var ve = e as DbEntityValidationException;
			//if(ve!=null)
			//	return new DbValidateException(
			//		"数据实体验证失败：" + FormatEntityValidationException(ve),
			//		ve
			//		);
			//var ce = e as System.Data.Entity.Infrastructure.DbUpdateConcurrencyException;
			//if (ce != null)
			//	return new DbUpdateConcurrencyException(
			//		"数据并发更新错误：" + FormatDbUpdateConcurrencyException(ce),
			//		ce);

			//var ue = e as System.Data.Entity.Infrastructure.DbUpdateException;
			//if (ue != null)
			//{
			//	if(ue.Entries.All(s=>s.State==EntityState.Added))
			//		return new DbDuplicatedKeyException(
			//			"主键或约束冲突：" + e.GetInnerExceptionMessage() + "：" + FormatDbUpdateException(ue),
			//			ue);
			//	else
			//		return new DbUpdateException(
			//			"数据更新失败：" + e.GetInnerExceptionMessage() + "：" + FormatDbUpdateException(ue),
			//			ue);
			//}


			//return new DbException("数据操作失败：" + e.GetInnerExceptionMessage(),e);
			return e;
		}
		public int SaveChanges()
		{
			if(IsChanged)
				try
				{
					var re=DbContext.SaveChanges();
					IsChanged = false;
					return re;
				}
				catch (Exception e)
				{
					throw MapException(e);
				}
			return 0;
		}
		public async Task<int> SaveChangesAsync()
		{
			if (IsChanged)
				try
				{
					var re= await DbContext.SaveChangesAsync();
					IsChanged = false;
					return re;
				}
				catch (Exception e)
				{
					throw MapException(e);
				}
			return 0;
		}
		
		public IDataSetProvider<T> SetProvider<T>() where T:class
		{
			return new DataSetProvider<T>(this);
		}


		public IContextQueryable<T> CreateQueryable<T>(IQueryable<T> Query)
		{
			return new DbQueryable<T>(this, Query);
		}

		public IOrderedContextQueryable<T> CreateOrderedQueryable<T>(IOrderedQueryable<T> Query)
		{
			return new DbOrderedQueryable<T>(this, Query);
		}


		
        public object GetEntityOriginalValue(object Entity,string Field) 
        {
			throw new NotSupportedException();
            //var osm=((IObjectContextAdapter)this).ObjectContext.ObjectStateManager;
            //var ov=osm.GetObjectStateEntry(Entity).OriginalValues;
            //var idx=ov.GetOrdinal(Field);
            //return ov.GetValue(idx);
        }
        public string GetEntitySetName<T>() where T :class
        {
			throw new NotSupportedException();
			//var type = typeof(T);
   //         var attr = type.GetCustomAttribute<TableAttribute>();
   //         if (attr != null)
   //             return attr.Name;

   //         string name = (this as IObjectContextAdapter).ObjectContext
   //             .CreateObjectSet<T>()
   //             .EntitySet.Name;
   //         return name;
        }
        //class Updater<T> : 
        //    IFieldUpdater<T>
        //    where T:class
        //{
        //    public DbEntityEntry<T> entry;
        //    public IFieldUpdater<T> Update<P>(System.Linq.Expressions.Expression<Func<T, P>> field)
        //    {
        //        entry.Property(field).IsModified = true;
        //        return this;
        //    }
        //}
        public void UpdateFields<T>(
            T item, 
            Func<IFieldUpdater<T>, IFieldUpdater<T>> updater
            ) where T : class
        {
			throw new NotSupportedException();
            //Set<T>().Attach(item);
            //var u = new Updater<T> { entry = Entry(item) };
            //updater(u);
        }

		public void Dispose()
		{
			
			DbContext.Dispose();
		}

		public void ClearTrackingEntities()
		{
			var changedEntriesCopy = DbContext.ChangeTracker.Entries()
			   .Where(e => e.State == EntityState.Added ||
						   e.State == EntityState.Modified ||
						   e.State == EntityState.Deleted || 
						   e.State==EntityState.Unchanged
						   )
			   .ToList();
			foreach (var entity in changedEntriesCopy)
			{
				DbContext.Entry(entity.Entity).State = EntityState.Detached;
			}
		}
		private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

		private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");

		private static readonly PropertyInfo NodeTypeProviderField = QueryCompilerTypeInfo.DeclaredProperties.Single(x => x.Name == "NodeTypeProvider");

		private static readonly MethodInfo CreateQueryParserMethod = QueryCompilerTypeInfo.DeclaredMethods.First(x => x.Name == "CreateQueryParser");

		private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");

		private static readonly PropertyInfo DatabaseDependenciesField = typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

		public IEnumerable<string> GetUnderlingCommandTexts<TEntity>(IContextQueryable<TEntity> Queryable) where TEntity:class
		{
			var dbq = Queryable as DbQueryable<TEntity>;
			if (dbq == null)
				return Enumerable.Empty<string>();
			var query = dbq.Queryable;
			//if (!(query is EntityQueryable<TEntity>) && !(query is InternalDbSet<TEntity>))
			//{
			//	throw new ArgumentException("Invalid query");
			//}

			var queryCompiler = (QueryCompiler)QueryCompilerField.GetValue(query.Provider);
			var nodeTypeProvider = (INodeTypeProvider)NodeTypeProviderField.GetValue(queryCompiler);
			var parser = (IQueryParser)CreateQueryParserMethod.Invoke(queryCompiler, new object[] { nodeTypeProvider });
			var queryModel = parser.GetParsedQuery(query.Expression);
			var database = DataBaseField.GetValue(queryCompiler);
			var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.GetValue(database);
			var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
			var modelVisitor = (RelationalQueryModelVisitor)queryCompilationContext.CreateQueryModelVisitor();
			modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
			return modelVisitor.Queries.Select(q => q.ToString());
		}

		public DbConnection GetDbConnection()
		{
			return DbContext.Database.GetDbConnection();
		}
	}
	
}
