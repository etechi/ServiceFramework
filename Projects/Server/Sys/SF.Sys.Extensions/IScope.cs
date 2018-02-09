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

using SF.Sys.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys
{
	public interface IScope<TContext>
	{
		Task Use(Func<TContext, CancellationToken, Task> Callback,CancellationToken cancellationToken=default(CancellationToken));
	}

	public static class Scope
	{

		public static async Task<TResult> Use<TContext, TResult>(
			this IScope<TContext> Scope,
			Func<TContext, CancellationToken, Task<TResult>> Callback,
			CancellationToken cancellationToken = default(CancellationToken)
			)
		{
			TResult re = default(TResult);
			await Scope.Use(async (ctx, ct) =>
			{
				re = await Callback(ctx, ct);
			}, cancellationToken);
			return re;
		}

		public static Task<TResult> Use<TContext, TResult>(
			this IScope<TContext> Scope,
			Func<TContext, Task<TResult>> Callback,
			CancellationToken cancellationToken = default(CancellationToken)
			)
			=> Use(Scope, (ctx, ct) => Callback(ctx), cancellationToken);

		public static Task Use<TContext>(
			this IScope<TContext> Scope,
			Func<TContext, Task> Callback,
			CancellationToken cancellationToken = default(CancellationToken)
			)
			=> Scope.Use((ctx, ct) => Callback(ctx), cancellationToken);

		class DelegateScope<TContext> : IScope<TContext>
		{
			Func<Func<TContext, CancellationToken, Task>, CancellationToken, Task> Func { get; }
			public DelegateScope(Func<Func<TContext, CancellationToken, Task>, CancellationToken, Task> Func)
			{
				this.Func = Func;
			}
#pragma warning disable RECS0146 // Member hides static member from outer class
			public Task Use(Func<TContext, CancellationToken, Task> Callback, CancellationToken cancellationToken)
#pragma warning restore RECS0146 // Member hides static member from outer class
			{
				return Func(
					Callback,
					cancellationToken
					);
			}
		}
		class ValueScope<TContext> : IScope<TContext>
		{
			TContext Value { get; }
			public ValueScope(TContext Value)
			{
				this.Value = Value;
			}
#pragma warning disable RECS0146 // Member hides static member from outer class
			public Task Use(Func<TContext, CancellationToken, Task> Callback, CancellationToken cancellactionToken)
#pragma warning restore RECS0146 // Member hides static member from outer class
			{
				return Callback(Value, cancellactionToken);
			}
		}
		public static IScope<TResult> From<TResult>(TResult Result)
			=> new ValueScope<TResult>(Result);

		public static IScope<TResult> Create<TResult>(Func<Func<TResult, CancellationToken, Task>, CancellationToken, Task> Func)
			=> new DelegateScope<TResult>(Func);

		public static Task<TResult> Result<TResult>(this IScope<TResult> scope, CancellationToken cancellationToken = default(CancellationToken))
		{
			return scope.Use<TResult, TResult>((ctx, ct) => Task.FromResult(ctx), cancellationToken);
		}



		public static IScope<TResult> Using<TSource, TResult>(this IScope<TSource> source, Func<TSource, TResult> selector)
			where TResult : IDisposable
			=> Create<TResult>(
				(cb, ct) =>
					source.Use(async (ictx, ict) =>
					{
						using (var re = selector(ictx))
							await cb(re, ict);
					},
					ct)
				);
		public static IScope<TResult> Convert<TSource, TResult>(
			this IScope<TSource> source,
			Func<TSource, Func<TResult, CancellationToken, Task>, CancellationToken, Task> Func
			)
			=> Create<TResult>(
				(cb, ct) =>
					source.Use(
						(ictx, ict) =>
							Func(ictx, cb, ict)
					,
					ct
					)
				);
		public static IScope<TSource> Wrap<TSource>(
			this IScope<TSource> source,
			Func<TSource, Func<TSource, CancellationToken, Task>, CancellationToken, Task> Func
			)
			=> Create<TSource>(
				(cb, ct) =>
					source.Use(
						(ictx, ict) =>
							Func(ictx, cb, ict)
					,
					ct
					)
				);

		

		public static IScope<TResult> Select<TSource, TResult>(this IScope<TSource> source, Func<TSource, int, TResult> selector)
			=> Create<TResult>(
				(cb, ct) =>
				source.Use((ictx, ict) =>
					cb(selector(ictx, 0), ict),
					ct
					)
				);

		public static IScope<TResult> Select<TSource, TResult>(this IScope<TSource> source, Func<TSource, TResult> selector)
			=> Create<TResult>(
				(cb, ct) =>
					source.Use((ictx, ict) =>
						cb(selector(ictx), ict),
						ct
						)
				);

		public static IScope<TResult> SelectMany<TSource, TResult>(this IScope<TSource> source, Func<TSource, int, IScope<TResult>> selector)
			=> Create<TResult>(
				(cb, ct) =>
					source.Use((ictx, ict) =>
						selector(ictx, 0).Use((iictx, iict) =>
							 cb(iictx, iict),
							ict
						),
						ct
					)
				);
		public static IScope<TResult> SelectMany<TSource, TResult>(this IScope<TSource> source, Func<TSource, IScope<TResult>> selector)
			=> Create<TResult>(
				(cb, ct) =>
					source.Use((ictx, ict) =>
						selector(ictx).Use((iictx, iict) =>
							 cb(iictx, iict),
							ict
						),
						ct
					)
				);

		public static IScope<TResult> SelectMany<TSource, TCollection, TResult>(this IScope<TSource> source, Func<TSource, int, IScope<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
			=> Create<TResult>(
				(cb, ct) =>
					source.Use((ictx, ict) =>
						collectionSelector(ictx, 0).Use(
							(iictx, iict) =>
							 cb(resultSelector(ictx, iictx), iict),
							ict
						),
						ct
					)
				);
		public static IScope<TResult> SelectMany<TSource, TCollection, TResult>(this IScope<TSource> source, Func<TSource, IScope<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
			=> Create<TResult>(
				(cb, ct) =>
					source.Use((ictx, ict) =>
						collectionSelector(ictx).Use(
							(iictx, iict) =>
							  cb(resultSelector(ictx, iictx), iict),
							ict
						),
						ct
					)
				);

		public static IScope<TResult> SelectMany<TSource, TResult>(this IScope<TSource> source, Func<TSource, int, Task<TResult>> selector)
			=> Create<TResult>(
				(cb, ct) =>
					source.Use(async (ictx, ict) =>
					{
						var re = await selector(ictx, 0);
						if (re is IDisposable dp)
							using (dp)
								await cb(re, ict);
						else
							await cb(re, ict);
					},
					ct
					)
				);
		public static IScope<TResult> SelectMany<TSource, TResult>(this IScope<TSource> source, Func<TSource, Task<TResult>> selector)
			=> Create<TResult>(
				(cb, ct) =>
					source.Use(async (ictx, ict) =>
						{
							var re = await selector(ictx);
							if (re is IDisposable dp)
								using (dp)
									await cb(re, ict);
							else
								await cb(re, ict);
						},
						ct
					)
				);

		public static IScope<TResult> SelectMany<TSource, TCollection, TResult>(this IScope<TSource> source, Func<TSource, int, Task<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
			=> Create<TResult>(
				(cb, ct) =>
					source.Use(async (ictx, ict) =>
						{
							var re = await collectionSelector(ictx,0);
							if (re is IDisposable dp)
								using (dp)
									await cb(resultSelector(ictx, re), ict);
							else
								await cb(resultSelector(ictx, re), ict);
						},
						ct
					)
				);
		public static IScope<TResult> SelectMany<TSource, TCollection, TResult>(this IScope<TSource> source, Func<TSource, Task<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
			=> Create<TResult>(
				(cb,ct) =>
					source.Use(async (ictx,ict) =>
						{
							var re = await collectionSelector(ictx);
							if (re is IDisposable dp)
								using (dp)
									await cb(resultSelector(ictx, re), ict);
							else
								await cb(resultSelector(ictx, re), ict);
						},
						ct
					)
				);


		
		public static IScope<TSource> Where<TSource>(this IScope<TSource> source, Func<TSource, int, bool> predicate)
			=> Create<TSource>(
				(cb,ct) =>
					source.Use((ictx,ict) =>
						{
							if (predicate(ictx, 0))
								return cb(ictx,ict);
							return Task.CompletedTask;
						},
						ct
					)
			);
		public static IScope<TSource> Where<TSource>(this IScope<TSource> source, Func<TSource, bool> predicate)
			=> Create<TSource>(
				(cb,ct) =>
					source.Use((ictx,ict) =>
					{
						if (predicate(ictx))
							return cb(ictx,ict);
						return Task.CompletedTask;
					},
					ct
					)
			);

	}

}
