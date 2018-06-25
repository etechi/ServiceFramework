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
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading;
using System.Reflection;
using SF.Sys.Collections.Generic;
using SF.Sys.Linq;

namespace System.Linq
{

	public static class AsyncQueryableExtensions
	{
		

		private static readonly MethodInfo _any = GetMethod("Any", 0, null);

		private static readonly MethodInfo _anyPredicate = GetMethod("Any", 1, null);

		private static readonly MethodInfo _allPredicate = GetMethod("All", 1, null);

		private static readonly MethodInfo _count = GetMethod("Count", 0, null);

		private static readonly MethodInfo _countPredicate = GetMethod("Count", 1, null);

		private static readonly MethodInfo _longCount = GetMethod("LongCount", 0, null);

		private static readonly MethodInfo _longCountPredicate = GetMethod("LongCount", 1, null);

		private static readonly MethodInfo _first = GetMethod("First", 0, null);

		private static readonly MethodInfo _firstPredicate = GetMethod("First", 1, null);

		private static readonly MethodInfo _firstOrDefault = GetMethod("FirstOrDefault", 0, null);

		private static readonly MethodInfo _firstOrDefaultPredicate = GetMethod("FirstOrDefault", 1, null);

		private static readonly MethodInfo _last = GetMethod("Last", 0, null);

		private static readonly MethodInfo _lastPredicate = GetMethod("Last", 1, null);

		private static readonly MethodInfo _lastOrDefault = GetMethod("LastOrDefault", 0, null);

		private static readonly MethodInfo _lastOrDefaultPredicate = GetMethod("LastOrDefault", 1, null);

		private static readonly MethodInfo _single = GetMethod("Single", 0, null);

		private static readonly MethodInfo _singlePredicate = GetMethod("Single", 1, null);

		private static readonly MethodInfo _singleOrDefault = GetMethod("SingleOrDefault", 0, null);

		private static readonly MethodInfo _singleOrDefaultPredicate = GetMethod("SingleOrDefault", 1, null);

		private static readonly MethodInfo _min = GetMethod("Min", 0, (MethodInfo mi) => mi.IsGenericMethod);

		private static readonly MethodInfo _minSelector = GetMethod("Min", 1, (MethodInfo mi) => mi.IsGenericMethod);

		private static readonly MethodInfo _max = GetMethod("Max", 0, (MethodInfo mi) => mi.IsGenericMethod);

		private static readonly MethodInfo _maxSelector = GetMethod("Max", 1, (MethodInfo mi) => mi.IsGenericMethod);

		private static readonly MethodInfo _sumDecimal = GetMethod<decimal>("Sum", 0, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumNullableDecimal = GetMethod<decimal?>("Sum", 0, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumDecimalSelector = GetMethod<decimal>("Sum", 1, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumNullableDecimalSelector = GetMethod<decimal?>("Sum", 1, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumInt = GetMethod<int>("Sum", 0, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumNullableInt = GetMethod<int?>("Sum", 0, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumIntSelector = GetMethod<int>("Sum", 1, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumNullableIntSelector = GetMethod<int?>("Sum", 1, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumLong = GetMethod<long>("Sum", 0, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumNullableLong = GetMethod<long?>("Sum", 0, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumLongSelector = GetMethod<long>("Sum", 1, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumNullableLongSelector = GetMethod<long?>("Sum", 1, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumDouble = GetMethod<double>("Sum", 0, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumNullableDouble = GetMethod<double?>("Sum", 0, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumDoubleSelector = GetMethod<double>("Sum", 1, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumNullableDoubleSelector = GetMethod<double?>("Sum", 1, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumFloat = GetMethod<float>("Sum", 0, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumNullableFloat = GetMethod<float?>("Sum", 0, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumFloatSelector = GetMethod<float>("Sum", 1, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _sumNullableFloatSelector = GetMethod<float?>("Sum", 1, (Func<MethodInfo, bool>)null);

		private static readonly MethodInfo _averageDecimal = GetAverageMethod<decimal, decimal>(0);

		private static readonly MethodInfo _averageNullableDecimal = GetAverageMethod<decimal?, decimal?>(0);

		private static readonly MethodInfo _averageDecimalSelector = GetAverageMethod<decimal, decimal>(1);

		private static readonly MethodInfo _averageNullableDecimalSelector = GetAverageMethod<decimal?, decimal?>(1);

		private static readonly MethodInfo _averageInt = GetAverageMethod<int, double>(0);

		private static readonly MethodInfo _averageNullableInt = GetAverageMethod<int?, double?>(0);

		private static readonly MethodInfo _averageIntSelector = GetAverageMethod<int, double>(1);

		private static readonly MethodInfo _averageNullableIntSelector = GetAverageMethod<int?, double?>(1);

		private static readonly MethodInfo _averageLong = GetAverageMethod<long, double>(0);

		private static readonly MethodInfo _averageNullableLong = GetAverageMethod<long?, double?>(0);

		private static readonly MethodInfo _averageLongSelector = GetAverageMethod<long, double>(1);

		private static readonly MethodInfo _averageNullableLongSelector = GetAverageMethod<long?, double?>(1);

		private static readonly MethodInfo _averageDouble = GetAverageMethod<double, double>(0);

		private static readonly MethodInfo _averageNullableDouble = GetAverageMethod<double?, double?>(0);

		private static readonly MethodInfo _averageDoubleSelector = GetAverageMethod<double, double>(1);

		private static readonly MethodInfo _averageNullableDoubleSelector = GetAverageMethod<double?, double?>(1);

		private static readonly MethodInfo _averageFloat = GetAverageMethod<float, float>(0);

		private static readonly MethodInfo _averageNullableFloat = GetAverageMethod<float?, float?>(0);

		private static readonly MethodInfo _averageFloatSelector = GetAverageMethod<float, float>(1);

		private static readonly MethodInfo _averageNullableFloatSelector = GetAverageMethod<float?, float?>(1);

		private static readonly MethodInfo _contains = GetMethod("Contains", 1, null);


		/// <summary>
		///     Asynchronously determines whether a sequence contains any elements.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to check for being empty.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains <c>true</c> if the source sequence contains any elements; otherwise, <c>false</c>.
		/// </returns>
		public static Task<bool> AnyAsync<TSource>( this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<TSource, bool>(_any, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously determines whether any element of a sequence satisfies a condition.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> whose elements to test for a condition.
		/// </param>
		/// <param name="predicate"> A function to test each element for a condition. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains <c>true</c> if any elements in the source sequence pass the test in the specified
		///     predicate; otherwise, <c>false</c>.
		/// </returns>
		public static Task<bool> AnyAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, bool>(_anyPredicate, source, predicate, cancellationToken);
		}

		/// <summary>
		///     Asynchronously determines whether all the elements of a sequence satisfy a condition.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> whose elements to test for a condition.
		/// </param>
		/// <param name="predicate"> A function to test each element for a condition. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains <c>true</c> if every element of the source sequence passes the test in the specified
		///     predicate; otherwise, <c>false</c>.
		/// </returns>
		public static Task<bool> AllAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, bool>(_allPredicate, source, predicate, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the number of elements in a sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the number of elements in the input sequence.
		/// </returns>
		public static Task<int> CountAsync<TSource>( this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<TSource, int>(_count, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the number of elements in a sequence that satisfy a condition.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.
		/// </param>
		/// <param name="predicate"> A function to test each element for a condition. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the number of elements in the sequence that satisfy the condition in the predicate
		///     function.
		/// </returns>
		public static Task<int> CountAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, int>(_countPredicate, source, predicate, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns an <see cref="T:System.Int64" /> that represents the total number of elements in a sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the number of elements in the input sequence.
		/// </returns>
		public static Task<long> LongCountAsync<TSource>( this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<TSource, long>(_longCount, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns an <see cref="T:System.Int64" /> that represents the number of elements in a sequence
		///     that satisfy a condition.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.
		/// </param>
		/// <param name="predicate"> A function to test each element for a condition. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the number of elements in the sequence that satisfy the condition in the predicate
		///     function.
		/// </returns>
		public static Task<long> LongCountAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, long>(_longCountPredicate, source, predicate, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the first element of a sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the first element in <paramref name="source" />.
		/// </returns>
		public static Task<TSource> FirstAsync<TSource>( this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<TSource, TSource>(_first, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the first element of a sequence that satisfies a specified condition.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.
		/// </param>
		/// <param name="predicate"> A function to test each element for a condition. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the first element in <paramref name="source" /> that passes the test in
		///     <paramref name="predicate" />.
		/// </returns>
		public static Task<TSource> FirstAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, TSource>(_firstPredicate, source, predicate, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the first element of a sequence, or a default value if the sequence contains no elements.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains <c>default</c> ( <typeparamref name="TSource" /> ) if
		///     <paramref name="source" /> is empty; otherwise, the first element in <paramref name="source" />.
		/// </returns>
		public static Task<TSource> FirstOrDefaultAsync<TSource>( this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<TSource, TSource>(_firstOrDefault, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the first element of a sequence that satisfies a specified condition
		///     or a default value if no such element is found.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.
		/// </param>
		/// <param name="predicate"> A function to test each element for a condition. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains <c>default</c> ( <typeparamref name="TSource" /> ) if <paramref name="source" />
		///     is empty or if no element passes the test specified by <paramref name="predicate" /> ; otherwise, the first
		///     element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.
		/// </returns>
		public static Task<TSource> FirstOrDefaultAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, TSource>(_firstOrDefaultPredicate, source, predicate, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the last element of a sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to return the last element of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the last element in <paramref name="source" />.
		/// </returns>
		public static Task<TSource> LastAsync<TSource>( this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<TSource, TSource>(_last, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the last element of a sequence that satisfies a specified condition.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to return the last element of.
		/// </param>
		/// <param name="predicate"> A function to test each element for a condition. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the last element in <paramref name="source" /> that passes the test in
		///     <paramref name="predicate" />.
		/// </returns>
		public static Task<TSource> LastAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, TSource>(_lastPredicate, source, predicate, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the last element of a sequence, or a default value if the sequence contains no elements.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to return the last element of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains <c>default</c> ( <typeparamref name="TSource" /> ) if
		///     <paramref name="source" /> is empty; otherwise, the last element in <paramref name="source" />.
		/// </returns>
		public static Task<TSource> LastOrDefaultAsync<TSource>( this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<TSource, TSource>(_lastOrDefault, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the last element of a sequence that satisfies a specified condition
		///     or a default value if no such element is found.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to return the last element of.
		/// </param>
		/// <param name="predicate"> A function to test each element for a condition. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains <c>default</c> ( <typeparamref name="TSource" /> ) if <paramref name="source" />
		///     is empty or if no element passes the test specified by <paramref name="predicate" /> ; otherwise, the last
		///     element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.
		/// </returns>
		public static Task<TSource> LastOrDefaultAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, TSource>(_lastOrDefaultPredicate, source, predicate, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the only element of a sequence, and throws an exception
		///     if there is not exactly one element in the sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the single element of the input sequence.
		/// </returns>
		public static Task<TSource> SingleAsync<TSource>( this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<TSource, TSource>(_single, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the only element of a sequence that satisfies a specified condition,
		///     and throws an exception if more than one such element exists.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
		/// </param>
		/// <param name="predicate"> A function to test an element for a condition. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the single element of the input sequence that satisfies the condition in
		///     <paramref name="predicate" />.
		/// </returns>
		public static Task<TSource> SingleAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, TSource>(_singlePredicate, source, predicate, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the only element of a sequence, or a default value if the sequence is empty;
		///     this method throws an exception if there is more than one element in the sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the single element of the input sequence, or <c>default</c> (
		///     <typeparamref name="TSource" />)
		///     if the sequence contains no elements.
		/// </returns>
		public static Task<TSource> SingleOrDefaultAsync<TSource>( this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<TSource, TSource>(_singleOrDefault, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the only element of a sequence that satisfies a specified condition or
		///     a default value if no such element exists; this method throws an exception if more than one element
		///     satisfies the condition.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
		/// </param>
		/// <param name="predicate"> A function to test an element for a condition. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the single element of the input sequence that satisfies the condition in
		///     <paramref name="predicate" />, or <c>default</c> ( <typeparamref name="TSource" /> ) if no such element is found.
		/// </returns>
		public static Task<TSource> SingleOrDefaultAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, TSource>(_singleOrDefaultPredicate, source, predicate, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the minimum value of a sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to determine the minimum of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the minimum value in the sequence.
		/// </returns>
		public static Task<TSource> MinAsync<TSource>( this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<TSource, TSource>(_min, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously invokes a projection function on each element of a sequence and returns the minimum resulting value.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <typeparam name="TResult">
		///     The type of the value returned by the function represented by <paramref name="selector" /> .
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to determine the minimum of.
		/// </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the minimum value in the sequence.
		/// </returns>
		public static Task<TResult> MinAsync<TSource, TResult>( this IQueryable<TSource> source,  Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, TResult>(_minSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously returns the maximum value of a sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to determine the maximum of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the maximum value in the sequence.
		/// </returns>
		public static Task<TSource> MaxAsync<TSource>( this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<TSource, TSource>(_max, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously invokes a projection function on each element of a sequence and returns the maximum resulting value.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <typeparam name="TResult">
		///     The type of the value returned by the function represented by <paramref name="selector" /> .
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to determine the maximum of.
		/// </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the maximum value in the sequence.
		/// </returns>
		public static Task<TResult> MaxAsync<TSource, TResult>( this IQueryable<TSource> source,  Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, TResult>(_maxSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the sum of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the values in the sequence.
		/// </returns>
		public static Task<decimal> SumAsync( this IQueryable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<decimal, decimal>(_sumDecimal, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the sum of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the values in the sequence.
		/// </returns>
		public static Task<decimal?> SumAsync( this IQueryable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<decimal?, decimal?>(_sumNullableDecimal, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of the sequence of values that is obtained by invoking a projection function on
		///     each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values of type <typeparamref name="TSource" />.
		/// </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the projected values..
		/// </returns>
		public static Task<decimal> SumAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, decimal>(_sumDecimalSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of the sequence of values that is obtained by invoking a projection function on
		///     each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values of type <typeparamref name="TSource" />.
		/// </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the projected values..
		/// </returns>
		public static Task<decimal?> SumAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, decimal?>(_sumNullableDecimalSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the sum of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the values in the sequence.
		/// </returns>
		public static Task<int> SumAsync( this IQueryable<int> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<int, int>(_sumInt, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the sum of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the values in the sequence.
		/// </returns>
		public static Task<int?> SumAsync( this IQueryable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<int?, int?>(_sumNullableInt, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of the sequence of values that is obtained by invoking a projection function on
		///     each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values of type <typeparamref name="TSource" />.
		/// </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the projected values..
		/// </returns>
		public static Task<int> SumAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, int>(_sumIntSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of the sequence of values that is obtained by invoking a projection function on
		///     each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values of type <typeparamref name="TSource" />.
		/// </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the projected values..
		/// </returns>
		public static Task<int?> SumAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, int?>(_sumNullableIntSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the sum of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the values in the sequence.
		/// </returns>
		public static Task<long> SumAsync( this IQueryable<long> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<long, long>(_sumLong, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the sum of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the values in the sequence.
		/// </returns>
		public static Task<long?> SumAsync( this IQueryable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<long?, long?>(_sumNullableLong, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of the sequence of values that is obtained by invoking a projection function on
		///     each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values of type <typeparamref name="TSource" />.
		/// </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the projected values..
		/// </returns>
		public static Task<long> SumAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, long>(_sumLongSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of the sequence of values that is obtained by invoking a projection function on
		///     each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values of type <typeparamref name="TSource" />.
		/// </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the projected values..
		/// </returns>
		public static Task<long?> SumAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, long?>(_sumNullableLongSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the sum of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the values in the sequence.
		/// </returns>
		public static Task<double> SumAsync( this IQueryable<double> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<double, double>(_sumDouble, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the sum of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the values in the sequence.
		/// </returns>
		public static Task<double?> SumAsync( this IQueryable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<double?, double?>(_sumNullableDouble, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of the sequence of values that is obtained by invoking a projection function on
		///     each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values of type <typeparamref name="TSource" />.
		/// </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the projected values..
		/// </returns>
		public static Task<double> SumAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, double>(_sumDoubleSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of the sequence of values that is obtained by invoking a projection function on
		///     each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values of type <typeparamref name="TSource" />.
		/// </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the projected values..
		/// </returns>
		public static Task<double?> SumAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, double?>(_sumNullableDoubleSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the sum of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the values in the sequence.
		/// </returns>
		public static Task<float> SumAsync( this IQueryable<float> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<float, float>(_sumFloat, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the sum of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the values in the sequence.
		/// </returns>
		public static Task<float?> SumAsync( this IQueryable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<float?, float?>(_sumNullableFloat, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of the sequence of values that is obtained by invoking a projection function on
		///     each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values of type <typeparamref name="TSource" />.
		/// </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the projected values..
		/// </returns>
		public static Task<float> SumAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, float>(_sumFloatSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the sum of the sequence of values that is obtained by invoking a projection function on
		///     each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values of type <typeparamref name="TSource" />.
		/// </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the sum of the projected values..
		/// </returns>
		public static Task<float?> SumAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, float?>(_sumNullableFloatSelector, source, selector, cancellationToken);
		}

		private static MethodInfo GetAverageMethod<TOperand, TResult>(int parameterCount = 0)
		{
			return GetMethod<TResult>("Average", parameterCount, (Func<MethodInfo, bool>)delegate (MethodInfo mi)
			{
				if (parameterCount == 0 && mi.GetParameters()[0].ParameterType == typeof(IQueryable<TOperand>))
				{
					return true;
				}
				if (mi.GetParameters().Length == 2)
				{
					return mi.GetParameters()[1].ParameterType.GenericTypeArguments[0].GenericTypeArguments[1] == typeof(TOperand);
				}
				return false;
			});
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the average of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the sequence of values.
		/// </returns>
		public static Task<decimal> AverageAsync( this IQueryable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<decimal, decimal>(_averageDecimal, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the average of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the sequence of values.
		/// </returns>
		public static Task<decimal?> AverageAsync( this IQueryable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<decimal?, decimal?>(_averageNullableDecimal, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values that is obtained
		///     by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" /> .
		/// </typeparam>
		/// <param name="source"> A sequence of values of type <typeparamref name="TSource" />. </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the projected values.
		/// </returns>
		public static Task<decimal> AverageAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, decimal>(_averageDecimalSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values that is obtained
		///     by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" /> .
		/// </typeparam>
		/// <param name="source"> A sequence of values of type <typeparamref name="TSource" />. </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the projected values.
		/// </returns>
		public static Task<decimal?> AverageAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, decimal?>(_averageNullableDecimalSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the average of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the sequence of values.
		/// </returns>
		public static Task<double> AverageAsync( this IQueryable<int> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<int, double>(_averageInt, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the average of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the sequence of values.
		/// </returns>
		public static Task<double?> AverageAsync( this IQueryable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<int?, double?>(_averageNullableInt, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values that is obtained
		///     by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" /> .
		/// </typeparam>
		/// <param name="source"> A sequence of values of type <typeparamref name="TSource" />. </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the projected values.
		/// </returns>
		public static Task<double> AverageAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, double>(_averageIntSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values that is obtained
		///     by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" /> .
		/// </typeparam>
		/// <param name="source"> A sequence of values of type <typeparamref name="TSource" />. </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the projected values.
		/// </returns>
		public static Task<double?> AverageAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, double?>(_averageNullableIntSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the average of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the sequence of values.
		/// </returns>
		public static Task<double> AverageAsync( this IQueryable<long> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<long, double>(_averageLong, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the average of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the sequence of values.
		/// </returns>
		public static Task<double?> AverageAsync( this IQueryable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<long?, double?>(_averageNullableLong, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values that is obtained
		///     by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" /> .
		/// </typeparam>
		/// <param name="source"> A sequence of values of type <typeparamref name="TSource" />. </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the projected values.
		/// </returns>
		public static Task<double> AverageAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, double>(_averageLongSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values that is obtained
		///     by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" /> .
		/// </typeparam>
		/// <param name="source"> A sequence of values of type <typeparamref name="TSource" />. </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the projected values.
		/// </returns>
		public static Task<double?> AverageAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, double?>(_averageNullableLongSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the average of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the sequence of values.
		/// </returns>
		public static Task<double> AverageAsync( this IQueryable<double> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<double, double>(_averageDouble, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the average of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the sequence of values.
		/// </returns>
		public static Task<double?> AverageAsync( this IQueryable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<double?, double?>(_averageNullableDouble, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values that is obtained
		///     by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" /> .
		/// </typeparam>
		/// <param name="source"> A sequence of values of type <typeparamref name="TSource" />. </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the projected values.
		/// </returns>
		public static Task<double> AverageAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, double>(_averageDoubleSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values that is obtained
		///     by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" /> .
		/// </typeparam>
		/// <param name="source"> A sequence of values of type <typeparamref name="TSource" />. </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the projected values.
		/// </returns>
		public static Task<double?> AverageAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, double?>(_averageNullableDoubleSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the average of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the sequence of values.
		/// </returns>
		public static Task<float> AverageAsync( this IQueryable<float> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<float, float>(_averageFloat, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <param name="source">
		///     A sequence of values to calculate the average of.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the sequence of values.
		/// </returns>
		public static Task<float?> AverageAsync( this IQueryable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<float?, float?>(_averageNullableFloat, source, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values that is obtained
		///     by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" /> .
		/// </typeparam>
		/// <param name="source"> A sequence of values of type <typeparamref name="TSource" />. </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the projected values.
		/// </returns>
		public static Task<float> AverageAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, float>(_averageFloatSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously computes the average of a sequence of values that is obtained
		///     by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" /> .
		/// </typeparam>
		/// <param name="source"> A sequence of values of type <typeparamref name="TSource" />. </param>
		/// <param name="selector"> A projection function to apply to each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains the average of the projected values.
		/// </returns>
		public static Task<float?> AverageAsync<TSource>( this IQueryable<TSource> source,  Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			
			return ExecuteAsync<TSource, float?>(_averageNullableFloatSelector, source, selector, cancellationToken);
		}

		/// <summary>
		///     Asynchronously determines whether a sequence contains a specified element by using the default equality comparer.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.
		/// </param>
		/// <param name="item"> The object to locate in the sequence. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains <c>true</c> if the input sequence contains the specified value; otherwise, <c>false</c>.
		/// </returns>
		public static Task<bool> ContainsAsync<TSource>( this IQueryable<TSource> source,  TSource item, CancellationToken cancellationToken = default(CancellationToken))
		{
			
			return ExecuteAsync<TSource, bool>(_contains, source, (Expression)Expression.Constant(item, typeof(TSource)), cancellationToken);
		}
		public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(this IQueryable<TSource> source)
		{
			IAsyncEnumerable<TSource> result;
			if ((result = (source as IAsyncEnumerable<TSource>)) != null)
			{
				return result;
			}
			IAsyncEnumerableAccessor<TSource> asyncEnumerableAccessor;
			if ((asyncEnumerableAccessor = (source as IAsyncEnumerableAccessor<TSource>)) != null)
			{
				return asyncEnumerableAccessor.AsyncEnumerable;
			}
			throw new InvalidOperationException();
		}

		/// <summary>
		///     Asynchronously creates a <see cref="T:System.Collections.Generic.List`1" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it
		///     asynchronously.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to create a list from.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains a <see cref="T:System.Collections.Generic.List`1" /> that contains elements from the input sequence.
		/// </returns>
		public static Task<List<TSource>> ToListAsync<TSource>( this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return AsyncEnumerable.ToList<TSource>(source.AsAsyncEnumerable(), cancellationToken);
		}

		/// <summary>
		///     Asynchronously creates an array from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to create an array from.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains an array that contains elements from the input sequence.
		/// </returns>
		public static Task<TSource[]> ToArrayAsync<TSource>( this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return AsyncEnumerable.ToArray<TSource>(source.AsAsyncEnumerable(), cancellationToken);
		}


		/// <summary>
		///     Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it
		///     asynchronously
		///     according to a specified key selector function.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <typeparam name="TKey">
		///     The type of the key returned by <paramref name="keySelector" /> .
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.
		/// </param>
		/// <param name="keySelector"> A function to extract a key from each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains a <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains selected keys and values.
		/// </returns>
		public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>( this IQueryable<TSource> source,  Func<TSource, TKey> keySelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			
			
			return AsyncEnumerable.ToDictionary<TSource, TKey>(source.AsAsyncEnumerable(), keySelector, cancellationToken);
		}

		/// <summary>
		///     Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it
		///     asynchronously
		///     according to a specified key selector function and a comparer.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <typeparam name="TKey">
		///     The type of the key returned by <paramref name="keySelector" /> .
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.
		/// </param>
		/// <param name="keySelector"> A function to extract a key from each element. </param>
		/// <param name="comparer">
		///     An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains a <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains selected keys and values.
		/// </returns>
		public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>( this IQueryable<TSource> source,  Func<TSource, TKey> keySelector,  IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			
			
			
			return AsyncEnumerable.ToDictionary<TSource, TKey>(source.AsAsyncEnumerable(), keySelector, comparer, cancellationToken);
		}

		/// <summary>
		///     Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it
		///     asynchronously
		///     according to a specified key selector and an element selector function.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <typeparam name="TKey">
		///     The type of the key returned by <paramref name="keySelector" /> .
		/// </typeparam>
		/// <typeparam name="TElement">
		///     The type of the value returned by <paramref name="elementSelector" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.
		/// </param>
		/// <param name="keySelector"> A function to extract a key from each element. </param>
		/// <param name="elementSelector"> A transform function to produce a result element value from each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains a <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains values of type
		///     <typeparamref name="TElement" /> selected from the input sequence.
		/// </returns>
		public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>( this IQueryable<TSource> source,  Func<TSource, TKey> keySelector,  Func<TSource, TElement> elementSelector, CancellationToken cancellationToken = default(CancellationToken))
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			
			
			
			return AsyncEnumerable.ToDictionary<TSource, TKey, TElement>(source.AsAsyncEnumerable(), keySelector, elementSelector, cancellationToken);
		}

		/// <summary>
		///     Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it
		///     asynchronously
		///     according to a specified key selector function, a comparer, and an element selector function.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="TSource">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <typeparam name="TKey">
		///     The type of the key returned by <paramref name="keySelector" /> .
		/// </typeparam>
		/// <typeparam name="TElement">
		///     The type of the value returned by <paramref name="elementSelector" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.
		/// </param>
		/// <param name="keySelector"> A function to extract a key from each element. </param>
		/// <param name="elementSelector"> A transform function to produce a result element value from each element. </param>
		/// <param name="comparer">
		///     An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.
		/// </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		///     A task that represents the asynchronous operation.
		///     The task result contains a <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains values of type
		///     <typeparamref name="TElement" /> selected from the input sequence.
		/// </returns>
		public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>( this IQueryable<TSource> source,  Func<TSource, TKey> keySelector,  Func<TSource, TElement> elementSelector,  IEqualityComparer<TKey> comparer, CancellationToken cancellationToken = default(CancellationToken))
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			
			
			
			
			return AsyncEnumerable.ToDictionary<TSource, TKey, TElement>(source.AsAsyncEnumerable(), keySelector, elementSelector, comparer, cancellationToken);
		}

		/// <summary>
		///     Asynchronously enumerates the query results and performs the specified action on each element.
		/// </summary>
		/// <remarks>
		///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
		///     that any asynchronous operations have completed before calling another method on this context.
		/// </remarks>
		/// <typeparam name="T">
		///     The type of the elements of <paramref name="source" />.
		/// </typeparam>
		/// <param name="source">
		///     An <see cref="T:System.Linq.IQueryable`1" /> to enumerate.
		/// </param>
		/// <param name="action"> The action to perform on each element. </param>
		/// <param name="cancellationToken">
		///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
		/// </param>
		/// <returns> A task that represents the asynchronous operation. </returns>
		public static Task ForEachAsync<T>( this IQueryable<T> source,  Action<T> action, CancellationToken cancellationToken = default(CancellationToken))
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return AsyncEnumerable.ForEachAsync<T>(source.AsAsyncEnumerable(), action, cancellationToken);
		}

		private static Task<TResult> ExecuteAsync<TSource, TResult>(MethodInfo operatorMethodInfo, IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			IAsyncQueryProvider asyncQueryProvider;
			if ((asyncQueryProvider = (source.Provider as IAsyncQueryProvider)) != null)
			{
				if (operatorMethodInfo.IsGenericMethod)
				{
					operatorMethodInfo = operatorMethodInfo.MakeGenericMethod(typeof(TSource));
				}
				return asyncQueryProvider.ExecuteAsync<TResult>(Expression.Call(null, operatorMethodInfo, source.Expression), cancellationToken);
			}
			throw new InvalidOperationException();
		}

		private static Task<TResult> ExecuteAsync<TSource, TResult>(MethodInfo operatorMethodInfo, IQueryable<TSource> source, LambdaExpression expression, CancellationToken cancellationToken = default(CancellationToken))
		{
			return ExecuteAsync<TSource, TResult>(operatorMethodInfo, source, (Expression)Expression.Quote(expression), cancellationToken);
		}

		private static Task<TResult> ExecuteAsync<TSource, TResult>(MethodInfo operatorMethodInfo, IQueryable<TSource> source, Expression expression, CancellationToken cancellationToken = default(CancellationToken))
		{
			IAsyncQueryProvider asyncQueryProvider;
			if ((asyncQueryProvider = (source.Provider as IAsyncQueryProvider)) != null)
			{
				operatorMethodInfo = ((operatorMethodInfo.GetGenericArguments().Length == 2) ? operatorMethodInfo.MakeGenericMethod(typeof(TSource), typeof(TResult)) : operatorMethodInfo.MakeGenericMethod(typeof(TSource)));
				return asyncQueryProvider.ExecuteAsync<TResult>(Expression.Call(null, operatorMethodInfo, new Expression[2]
				{
				source.Expression,
				expression
				}), cancellationToken);
			}
			throw new InvalidOperationException();
		}

		private static MethodInfo GetMethod<TResult>(string name, int parameterCount = 0, Func<MethodInfo, bool> predicate = null)
		{
			return GetMethod(name, parameterCount, delegate (MethodInfo mi)
			{
				if (mi.ReturnType == typeof(TResult))
				{
					if (predicate != null)
					{
						return predicate(mi);
					}
					return true;
				}
				return false;
			});
		}

		private static MethodInfo GetMethod(string name, int parameterCount = 0, Func<MethodInfo, bool> predicate = null)
		{
			return typeof(Queryable).GetTypeInfo().GetDeclaredMethods(name).Single(delegate (MethodInfo mi)
			{
				if (mi.GetParameters().Length == parameterCount + 1)
				{
					if (predicate != null)
					{
						return predicate(mi);
					}
					return true;
				}
				return false;
			});
		}
	}
}
