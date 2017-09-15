using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SF.Data.EntityFrameworkCore
{
	class DataSetProvider<T> :  IDataSetProvider<T>,IDataSetMetadata
		where T:class
	{
		public DataSetProvider(EntityDbContextProvider Provider)
		{
			this.Set = Provider.DbContext.Set<T>();
			this.Provider = Provider;
		}

		DbSet<T> Set { get; }

		EntityDbContextProvider Provider { get; }

		public IDataContext Context => Provider.DataContext;

		string IDataSetMetadata.EntitySetName
		{
			get
			{
				return Provider.DbContext.Model.FindEntityType(typeof(T)).Relational().TableName;
			}
		}
		class EntityProperty : IEntityProperty
		{
			public string Name{get;set;}

			public PropertyInfo PropertyInfo{get;set;}
		}
		IEntityProperty[] IDataSetMetadata.Key
		{
			get
			{
				return Provider.DbContext
					.Model
					.FindEntityType(typeof(T))
					.FindPrimaryKey()
					.Properties.Select(p => new EntityProperty {
						Name = p.GetFieldName(),
						PropertyInfo = p.PropertyInfo
					})
					.ToArray();
			}
		}

		IEntityProperty[] IDataSetMetadata.Properties
		{
			get
			{
				return Provider.DbContext
					.Model
					.FindEntityType(typeof(T))
					.GetProperties()
					.Select(p => new EntityProperty {
						Name = p.GetFieldName(),
						PropertyInfo = p.PropertyInfo
					})
					.ToArray();
			}
		}

		public IDataSetMetadata Metadata
		{
			get
			{
				return this;
			}
		}

		public T Add(T Model)
		{
			var re = Set.Add(Model).Entity;
			Provider.SetChanged();
			return re;
		}

		public void AddRange(IEnumerable<T> Items)
		{
			Set.AddRange(Items);
			Provider.SetChanged();
		}


		public T Remove(T Model)
		{
			var re=Set.Remove(Model).Entity;
			Provider.SetChanged();
			return re;
		}

		public void RemoveRange(IEnumerable<T> Items)
		{
			Set.RemoveRange(Items);
			Provider.SetChanged();
		}


		public T Update(T item)
		{
			var re = Set.Update(item).Entity;
			Provider.SetChanged();
			return re;
		}

		
		public IContextQueryable<T> AsQueryable(bool ReadOnly)
		{

			IQueryable<T> query = Set;
			if (ReadOnly)
				query = query.AsNoTracking();
			return new DbQueryable<T>(Provider, query);
		}

		public Task<T> FindAsync(params object[] Idents)
		{
			return Set.FindAsync(Idents);
		}

		public Task<T> FindAsync(object Ident)
		{
			return Set.FindAsync(Ident);
		}

	
	}
	
}
