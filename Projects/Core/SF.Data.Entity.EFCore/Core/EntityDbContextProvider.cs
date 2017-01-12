using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace SF.Data.Entity.EntityFrameworkCore
{

	//public class ServiceProtocolDataIndexAttributeConvention : AttributeToColumnAnnotationConvention<ServiceProtocol.Data.Entity.IndexAttribute, IndexAnnotation>
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
		public EntityDbContextProvider(T Context):base(Context)
		{
		}
	}
	public class EntityDbContextProvider :
        IDataContextProvider,
        IDataStorageEngine,
        IDataContextExtension
    {
		public DbContext Context { get; }
		public EntityDbContextProvider(DbContext Context)
		{
			this.Context = Context;
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

		public IDataStorageEngine Engine
		{
			get
			{
				return this;
			}
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
			try
			{
				return Context.SaveChanges();
			}
			catch (Exception e)
			{
				throw MapException(e);
			}
		}
		public Task<int> SaveChangesAsync()
		{
			try
			{
				return Context.SaveChangesAsync();
			}
			catch (Exception e)
			{
				throw MapException(e);
			}
		}
		
		public IDataSetReadonly<T> ReadOnly<T>() where T:class
		{
            
			return new DataSetReadonly<T>(this, Context.Set<T>());
		}

		public IDataSetEditable<T> Editable<T>() where T : class
		{
			return new DataSetEditable<T>(this, Context.Set<T>());
		}

		

		public IContextQueryable<T> CreateQueryable<T>(IQueryable<T> Query)
		{
			return new DbQueryable<T>(this, Query);
		}

		public IOrderedContextQueryable<T> CreateOrderedQueryable<T>(IOrderedQueryable<T> Query)
		{
			return new DbOrderedQueryable<T>(this, Query);
		}


		class DataTransaction : IDataTransaction
		{
			Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction Transaction { get; }
			public DataTransaction(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction Transaction)
			{
				this.Transaction = Transaction;
			}

			public object UnderlyingTransaction
			{
				get
				{
					return Transaction;
				}
			}

			public void Commit()
			{
				Transaction.Commit();
			}

			public void Dispose()
			{
				Transaction.Dispose();
			}

			public void Rollback()
			{
				Transaction.Rollback();
			}
			public override string ToString()
			{
				return Transaction.ToString();
			}
			public override bool Equals(object obj)
			{
				var t = obj as DataTransaction;
				if (t == null) return false;
				return Transaction.Equals(t.Transaction);
			}
			public override int GetHashCode()
			{
				return Transaction.GetHashCode();
			}
		}
        Task<object> IDataStorageEngine.ExecuteCommandAsync(string Sql, params object[] Args)
        {
			throw new NotSupportedException();
            //return Database..ExecuteSqlCommandAsync(Sql, Args);
        }

        IDataTransaction IDataStorageEngine.BeginTransaction()
		{
			return new DataTransaction(Context.Database.BeginTransaction());
		}

		IDataTransaction IDataStorageEngine.BeginTransaction(IsolationLevel isolationLevel)
		{
			return new DataTransaction(Context.Database.BeginTransaction());
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
			Context.Dispose();
		}
    }
	
}
