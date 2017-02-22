﻿using System.Linq;

namespace SF.Data.Storage.EntityFrameworkCore
{
	class DbOrderedQueryable<T> :
		DbQueryable<T>,
		IOrderedContextQueryable<T>
	{

		public DbOrderedQueryable(EntityDbContextProvider Context, IQueryable<T> query):base(Context,query)
		{
		}

		IOrderedQueryable<T> IOrderedContextQueryable<T>.Queryable
		{
			get
			{
				return (IOrderedQueryable<T>)Queryable;
			}
		}
	}
	
}