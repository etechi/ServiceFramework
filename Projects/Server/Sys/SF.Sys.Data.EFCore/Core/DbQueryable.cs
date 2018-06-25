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

using SF.Sys.Collections.Generic;
using SF.Sys.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.Data.EntityFrameworkCore
{
	class DbQueryProvider : IAsyncQueryProvider, IEntityQueryProvider
	{
		public IDataContext Context { get; }
		Microsoft.EntityFrameworkCore.Query.Internal.IAsyncQueryProvider EFProvider { get; }

		public IDataContext DataContext => Context;

		public DbQueryProvider(IDataContext Context ,Microsoft.EntityFrameworkCore.Query.Internal.IAsyncQueryProvider EFProvider)
		{
			this.Context = Context;
			this.EFProvider = EFProvider;
		}
		public IQueryable CreateQuery(Expression expression)
		{
			return new DbQueryable(
				this,
				EFProvider.CreateQuery(expression)
				);
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new DbQueryable<TElement>(
				this,
				EFProvider.CreateQuery<TElement>(expression)
				);
		}

		public object Execute(Expression expression)
		{
			return EFProvider.Execute(expression);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return EFProvider.Execute<TResult>(expression);
		}

		public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
		{
			return EFProvider.ExecuteAsync<TResult>(expression);
		}

		public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
		{
			return EFProvider.ExecuteAsync<TResult>(expression,cancellationToken);
		}

		public IQueryable<T> Include<T>(IQueryable<T> source, string path) where T : class
		{
			return new DbQueryable<T>(
				source.Provider,
				Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(
					((DbQueryable<T>)source).InnerQueryable,
					path
					)
				);
		}

		public IQueryable<T> Include<T, TProperty>(IQueryable<T> source, Expression<Func<T, TProperty>> path) where T : class
		{
			return new DbQueryable<T>(
				source.Provider, 
				Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(
					((DbQueryable<T>)source).InnerQueryable,
					path
					)
				);
		}
	}
	class DbQueryable : IQueryable
	{
		public Type ElementType => InnerQueryable.ElementType;

		public Expression Expression => InnerQueryable.Expression;

		public IQueryProvider Provider { get; }

		public virtual IQueryable InnerQueryable { get; }

		public IEnumerator GetEnumerator()
		{
			return InnerQueryable.GetEnumerator();
		}
		internal DbQueryable(IQueryProvider Provider, IQueryable InnerQueryable)
		{
			this.Provider = Provider;
			this.InnerQueryable = InnerQueryable;
			//InternalQueryable = queryable;
			//Provider = new DbQueryProvider(
			//	Context,
			//	(Microsoft.EntityFrameworkCore.Query.Internal.IAsyncQueryProvider)queryable.Provider
			//	);
		}
	}
	class DbQueryable<T> : DbQueryable, IOrderedQueryable<T>, IQueryable<T>, IAsyncEnumerableAccessor<T>
	{
		public new IQueryable<T> InnerQueryable => (IQueryable<T>)base.InnerQueryable;

		public IAsyncEnumerable<T> AsyncEnumerable =>
			base.InnerQueryable is Microsoft.EntityFrameworkCore.Query.Internal.IAsyncEnumerableAccessor<T> accessor ?
			accessor.AsyncEnumerable :
			(IAsyncEnumerable<T>)base.InnerQueryable;

		internal DbQueryable(IQueryProvider Provider,IQueryable<T> InnerQueryable):
			base(Provider,InnerQueryable)
		{
		}
		
		public new IEnumerator<T> GetEnumerator()
		{
			return InnerQueryable.GetEnumerator();
		}
	}
	
}
