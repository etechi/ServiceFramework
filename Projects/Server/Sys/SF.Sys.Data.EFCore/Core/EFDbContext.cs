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
using System.Threading;

namespace SF.Sys.Data.EntityFrameworkCore
{

	
	public class EFDbContext :
		IDataContextProvider,
        IDataContextProviderExtension
    {
		public DbContext DbContext { get; }
		public bool IsChanged { get; private set; }
		internal void SetChanged()
		{
			
			IsChanged = true;
		}
		public EFDbContext(DbContext DbContext)
		{
			
			this.DbContext = DbContext;
			DbContext.ChangeTracker.AutoDetectChangesEnabled = false;
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
					throw EFException.MapException(e);
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
					throw EFException.MapException(e);
				}
			return 0;
		}
		public IDataSet<T> CreateDataSet<T>(IDataContext DataContext)where T : class
		{
			return new EFDataSet<T>(DataContext,this);
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
		//private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

		//private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");

		//private static readonly PropertyInfo NodeTypeProviderField = QueryCompilerTypeInfo.DeclaredProperties.Single(x => x.Name == "NodeTypeProvider");

		//private static readonly MethodInfo CreateQueryParserMethod = QueryCompilerTypeInfo.DeclaredMethods.First(x => x.Name == "CreateQueryParser");

		//private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");

		//private static readonly PropertyInfo DatabaseDependenciesField = typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

		public IEnumerable<string> GetUnderlingCommandTexts<TEntity>(IQueryable<TEntity> Queryable) where TEntity:class
		{
			return Enumerable.Empty<string>();
			//var dbq = Queryable as DbQueryable<TEntity>;
			//if (dbq == null)
			//	return Enumerable.Empty<string>();
			//var query = dbq.InnerQueryable;
			////if (!(query is EntityQueryable<TEntity>) && !(query is InternalDbSet<TEntity>))
			////{
			////	throw new ArgumentException("Invalid query");
			////}

			//var queryCompiler = (QueryCompiler)QueryCompilerField.GetValue(query.Provider);
			//var nodeTypeProvider = (INodeTypeProvider)NodeTypeProviderField.GetValue(queryCompiler);
			//var parser = (IQueryParser)CreateQueryParserMethod.Invoke(queryCompiler, new object[] { nodeTypeProvider });
			//var queryModel = parser.GetParsedQuery(query.Expression);
			//var database = DataBaseField.GetValue(queryCompiler);
			//var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.GetValue(database);
			//var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
			//var modelVisitor = (RelationalQueryModelVisitor)queryCompilationContext.CreateQueryModelVisitor();
			//modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
			//return modelVisitor.Queries.Select(q => q.ToString());
		}

		public DbConnection GetDbConnection()
		{
			return DbContext.Database.GetDbConnection();
		}

		class Transaction : IDataContextTransaction
		{
			IDbContextTransaction _Transaction;
			public Transaction(IDbContextTransaction Transaction)
			{
				_Transaction = Transaction;
			}

			public object RawTransaction => _Transaction.GetDbTransaction();

            public System.Data.IsolationLevel IsolationLevel => _Transaction.GetDbTransaction().IsolationLevel;

            public void Commit()
			{
				_Transaction.Commit();
			}

			public void Dispose()
			{
				Disposable.Release(ref _Transaction);
			}

			public void Rollback()
			{
				_Transaction.Rollback();
			}
		}

		public async Task<IDataContextTransaction> BeginTransaction(
			System.Data.IsolationLevel IsolationLevel,
			CancellationToken cancellationToken
			)
		{
			var tran = await DbContext.Database.BeginTransactionAsync(IsolationLevel, cancellationToken);
			return new Transaction(tran);
		}
	}
	
}
