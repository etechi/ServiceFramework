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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SF.Sys.Data;

namespace SF.Sys.Data.EntityFrameworkCore
{
	public class AsyncQueryableProvider : System.Linq.IAsyncQueryableProvider, IEntityQueryableProvider
	{
		AsyncQueryableProvider() { }
		public static AsyncQueryableProvider Instance { get; } = new AsyncQueryableProvider();

		public async Task<bool> AllAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			try
			{
				return await source.Queryable.AllAsync(predicate);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<bool> AllAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AllAsync(predicate, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<bool> AnyAsync<TSource>(IContextQueryable<TSource> source)
		{
			try
			{
				return await source.Queryable.AnyAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<bool> AnyAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AnyAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<bool> AnyAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			try
			{
				return await source.Queryable.AnyAsync(predicate);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<bool> AnyAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AnyAsync(predicate, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}


		public async Task<decimal> AverageAsync(IContextQueryable<decimal> source)
		{
			try
			{
				return await source.Queryable.AverageAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> AverageAsync(IContextQueryable<long> source)
		{
			try
			{
				return await source.Queryable.AverageAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> AverageAsync(IContextQueryable<int?> source)
		{
			try
			{
				return await source.Queryable.AverageAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float?> AverageAsync(IContextQueryable<float?> source)
		{
			try
			{
				return await source.Queryable.AverageAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> AverageAsync(IContextQueryable<int> source)
		{
			try
			{
				return await source.Queryable.AverageAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal?> AverageAsync(IContextQueryable<decimal?> source)
		{
			try
			{
				return await source.Queryable.AverageAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> AverageAsync(IContextQueryable<double> source)
		{
			try
			{
				return await source.Queryable.AverageAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> AverageAsync(IContextQueryable<double?> source)
		{
			try
			{
				return await source.Queryable.AverageAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float> AverageAsync(IContextQueryable<float> source)
		{
			try
			{
				return await source.Queryable.AverageAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> AverageAsync(IContextQueryable<long?> source)
		{
			try
			{
				return await source.Queryable.AverageAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> AverageAsync(IContextQueryable<int> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> AverageAsync(IContextQueryable<double> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> AverageAsync(IContextQueryable<long?> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> AverageAsync(IContextQueryable<double?> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal> AverageAsync(IContextQueryable<decimal> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> AverageAsync(IContextQueryable<long> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float?> AverageAsync(IContextQueryable<float?> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float> AverageAsync(IContextQueryable<float> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> AverageAsync(IContextQueryable<int?> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal?> AverageAsync(IContextQueryable<decimal?> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.AverageAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<bool> ContainsAsync<TSource>(IContextQueryable<TSource> source, TSource item)
		{
			try
			{
				return await source.Queryable.ContainsAsync(item);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<bool> ContainsAsync<TSource>(IContextQueryable<TSource> source, TSource item, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.ContainsAsync(item, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<int> CountAsync<TSource>(IContextQueryable<TSource> source)
		{
			try
			{
				return await source.Queryable.CountAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<int> CountAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			try
			{
				return await source.Queryable.CountAsync(predicate);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<int> CountAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.CountAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<int> CountAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.CountAsync(predicate, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> FirstAsync<TSource>(IContextQueryable<TSource> source)
		{
			try
			{
				return await source.Queryable.FirstAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> FirstAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			try
			{
				return await source.Queryable.FirstAsync(predicate);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> FirstAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.FirstAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> FirstAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.FirstAsync(predicate, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> FirstOrDefaultAsync<TSource>(IContextQueryable<TSource> source)
		{
			try
			{
				return await source.Queryable.FirstOrDefaultAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> FirstOrDefaultAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			try
			{
				return await source.Queryable.FirstAsync(predicate);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> FirstOrDefaultAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.FirstOrDefaultAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> FirstOrDefaultAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.FirstOrDefaultAsync(predicate, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task ForEachAsync<T>(IContextQueryable<T> source, Action<T> action)
		{
			try
			{
				await source.Queryable.ForEachAsync(action);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task ForEachAsync<T>(IContextQueryable<T> source, Action<T> action, CancellationToken cancellationToken)
		{
			try
			{
				await source.Queryable.ForEachAsync(action, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<long> LongCountAsync<TSource>(IContextQueryable<TSource> source)
		{
			try
			{
				return await source.Queryable.LongCountAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<long> LongCountAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			try
			{
				return await source.Queryable.LongCountAsync(predicate);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<long> LongCountAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.LongCountAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<long> LongCountAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.LongCountAsync(predicate, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> MaxAsync<TSource>(IContextQueryable<TSource> source)
		{
			try
			{
				return await source.Queryable.MaxAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> MaxAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.MaxAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TResult> MaxAsync<TSource, TResult>(IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
		{
			try
			{
				return await source.Queryable.MaxAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TResult> MaxAsync<TSource, TResult>(IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.MaxAsync(selector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> MinAsync<TSource>(IContextQueryable<TSource> source)
		{
			try
			{
				return await source.Queryable.MinAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> MinAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.MinAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TResult> MinAsync<TSource, TResult>(IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
		{
			try
			{
				return await source.Queryable.MinAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TResult> MinAsync<TSource, TResult>(IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.MinAsync(selector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> SingleAsync<TSource>(IContextQueryable<TSource> source)
		{
			try
			{
				return await source.Queryable.SingleAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> SingleAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SingleAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> SingleAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			try
			{
				return await source.Queryable.SingleAsync(predicate);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> SingleAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SingleAsync(predicate, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> SingleOrDefaultAsync<TSource>(IContextQueryable<TSource> source)
		{
			try
			{
				return await source.Queryable.SingleOrDefaultAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> SingleOrDefaultAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
		{
			try
			{
				return await source.Queryable.SingleOrDefaultAsync(predicate);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> SingleOrDefaultAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SingleOrDefaultAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource> SingleOrDefaultAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SingleOrDefaultAsync(predicate, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public IContextQueryable<TSource> Skip<TSource>(IContextQueryable<TSource> source, Expression<Func<int>> countAccessor)
		{
			throw new NotSupportedException();
			//{
			//	try
			//	{
			//		return await source.New(source.Queryable.Skip(countAccessor));
			//	}
			//	catch (Exception ex)
			//	{
			//		throw EFException.MapException(ex);
			//	}
			//}
		}

		public async Task<double?> SumAsync(IContextQueryable<double?> source)
		{
			try
			{
				return await source.Queryable.SumAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float> SumAsync(IContextQueryable<float> source)
		{
			try
			{
				return await source.Queryable.SumAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float?> SumAsync(IContextQueryable<float?> source)
		{
			try
			{
				return await source.Queryable.SumAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal?> SumAsync(IContextQueryable<decimal?> source)
		{
			try
			{
				return await source.Queryable.SumAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<int?> SumAsync(IContextQueryable<int?> source)
		{
			try
			{
				return await source.Queryable.SumAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal> SumAsync(IContextQueryable<decimal> source)
		{
			try
			{
				return await source.Queryable.SumAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<long> SumAsync(IContextQueryable<long> source)
		{
			try
			{
				return await source.Queryable.SumAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<int> SumAsync(IContextQueryable<int> source)
		{
			try
			{
				return await source.Queryable.SumAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> SumAsync(IContextQueryable<double> source)
		{
			try
			{
				return await source.Queryable.SumAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<long?> SumAsync(IContextQueryable<long?> source)
		{
			try
			{
				return await source.Queryable.SumAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<int> SumAsync(IContextQueryable<int> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float?> SumAsync(IContextQueryable<float?> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal?> SumAsync(IContextQueryable<decimal?> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<long?> SumAsync(IContextQueryable<long?> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<long> SumAsync(IContextQueryable<long> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<int?> SumAsync(IContextQueryable<int?> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> SumAsync(IContextQueryable<double> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float> SumAsync(IContextQueryable<float> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> SumAsync(IContextQueryable<double?> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal> SumAsync(IContextQueryable<decimal> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
		{
			try
			{
				return await source.Queryable.SumAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
		{
			try
			{
				return await source.Queryable.SumAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<int> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector)
		{
			try
			{
				return await source.Queryable.SumAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<long?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
		{
			try
			{
				return await source.Queryable.SumAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector)
		{
			try
			{
				return await source.Queryable.SumAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
		{
			try
			{
				return await source.Queryable.SumAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<int?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
		{
			try
			{
				return await source.Queryable.SumAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<long> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector)
		{
			try
			{
				return await source.Queryable.SumAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
		{
			try
			{
				return await source.Queryable.SumAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector)
		{
			try
			{
				return await source.Queryable.SumAsync(selector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<int?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(selector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(selector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(selector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<decimal?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(selector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<long?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(selector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<long> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(selector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<double> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(selector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<int> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(selector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(selector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<float> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.SumAsync(selector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public IContextQueryable<TSource> Take<TSource>(IContextQueryable<TSource> source, Expression<Func<int>> countAccessor)
		{
			throw new NotSupportedException();
			//{
			//	try
			//	{
			//		return await source.New(source.Queryable.Take(countAccessor));
			//	}
			//	catch (Exception ex)
			//	{
			//		throw EFException.MapException(ex);
			//	}
			//}
		}

		public async Task<TSource[]> ToArrayAsync<TSource>(IContextQueryable<TSource> source)
		{
			try
			{
				return await source.Queryable.ToArrayAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<TSource[]> ToArrayAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.ToArrayAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector)
		{
			try
			{
				return await source.Queryable.ToDictionaryAsync(keySelector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			try
			{
				return await source.Queryable.ToDictionaryAsync(keySelector, comparer);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.ToDictionaryAsync(keySelector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.ToDictionaryAsync(keySelector, comparer, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			try
			{
				return await source.Queryable.ToDictionaryAsync(keySelector, elementSelector);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}


		public async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			try
			{
				return await source.Queryable.ToDictionaryAsync(keySelector, elementSelector, comparer);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}
		public async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.ToDictionaryAsync(keySelector, elementSelector, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.ToDictionaryAsync(keySelector, elementSelector, comparer, cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<List<TSource>> ToListAsync<TSource>(IContextQueryable<TSource> source)
		{
			try
			{
				return await source.Queryable.ToListAsync();
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}

		public async Task<List<TSource>> ToListAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken)
		{
			try
			{
				return await source.Queryable.ToListAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw EFException.MapException(ex);
			}
		}



		public IContextQueryable<T> Include<T>(IContextQueryable<T> source, string path) where T : class
		{
				return source.New(source.Queryable.Include(path));
		}

		public IContextQueryable<T> Include<T, TProperty>(IContextQueryable<T> source, Expression<Func<T, TProperty>> path) where T : class
		{
			return source.New(source.Queryable.Include(path));
		}
	}
}
