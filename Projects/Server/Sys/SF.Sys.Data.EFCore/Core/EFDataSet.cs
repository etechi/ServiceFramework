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
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using SF.Sys.Data;

namespace SF.Sys.Data.EntityFrameworkCore
{
	class EFDataSet
	{
		public EFDbContext Context { get; }
		public EFDataSet(EFDbContext Context)
		{
			this.Context = Context;
		}
	}
	class EFDataSet<T> : EFDataSet, IDataSet<T>,IDataSetMetadata
		where T:class
	{
		public IDataContext DataContext { get; }
		public EFDataSet(IDataContext DataContext,EFDbContext Context):base(Context)
		{
			this.DataContext = DataContext;
			this.Set = Context.DbContext.Set<T>();
		}

		DbSet<T> Set { get; }

		IDataContext IDataSet.Context => DataContext;

		string IDataSetMetadata.EntitySetName
		{
			get
			{
				return Context.DbContext.Model.FindEntityType(typeof(T)).Relational().TableName;
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
				return Context.DbContext
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
				return Context.DbContext
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
			Context.SetChanged();
			return re;
		}

		public void AddRange(IEnumerable<T> Items)
		{
			Set.AddRange(Items);
			Context.SetChanged();
		}


		public T Remove(T Model)
		{
			var re=Set.Remove(Model).Entity;
			Context.SetChanged();
			return re;
		}

		public void RemoveRange(IEnumerable<T> Items)
		{
			Set.RemoveRange(Items);
			Context.SetChanged();
		}


		public T Update(T item)
		{
			var state = Context.DbContext.Entry(item).State;
			if (state == EntityState.Unchanged || state==EntityState.Detached)
			{
				var re=Set.Update(item).Entity;
				Context.SetChanged();
				return re;
			}
			return item;
		}

		
		public IQueryable<T> AsQueryable(bool ReadOnly)
		{

			IQueryable<T> query = Set;
			if (ReadOnly)
				query = query.AsNoTracking();
			return new DbQueryable<T>(
				new DbQueryProvider(DataContext, (Microsoft.EntityFrameworkCore.Query.Internal.IAsyncQueryProvider)query.Provider), 
				query
				);
		}

		public Task<T> FindAsync(params object[] Idents)
		{
			return Set.FindAsync(Idents);
		}

		public Task<T> FindAsync(object Ident)
		{
			return Set.FindAsync(Ident);
		}
        public IEnumerable<T> CachedEntities()
        {
            return Context.DbContext.ChangeTracker.Entries<T>().Where(e=>e.State!=EntityState.Detached).Select(e=>e.Entity);
        }
    }
	
}
