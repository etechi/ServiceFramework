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
			Func<TContext,CancellationToken, Task<TResult>> Callback,
			CancellationToken cancellationToken=default(CancellationToken)
			)
		{
			TResult re = default(TResult);
			await Scope.Use(async (ctx,ct) =>
			{
				re = await Callback(ctx,ct);
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
			public DelegateScope(Func<Func<TContext, CancellationToken,Task>, CancellationToken, Task> Func)
			{
				this.Func = Func;
			}
#pragma warning disable RECS0146 // Member hides static member from outer class
			public Task Use(Func<TContext, CancellationToken,Task> Callback, CancellationToken cancellationToken)
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

		public static IScope<TResult> Create<TResult>(Func<Func<TResult, CancellationToken,Task>, CancellationToken, Task> Func)
			=> new DelegateScope<TResult>(Func);

		public static Task<TResult> Result<TResult>(IScope<TResult> scope,  CancellationToken cancellationToken=default(CancellationToken))
		{
			return scope.Use<TResult,TResult>((ctx, ct) => Task.FromResult(ctx), cancellationToken);
		}

		public static IScope<TResult> Cached<TResult>(this IScope<TResult> scope)
		{
			var result = default(TResult);
			var uninited = true;
			var ss = new SyncScope();

			return Create<TResult>(async (cb,ct) =>
			{
				if (uninited)
					result = await ss.Sync(async () =>
					{
						if (uninited)
						{
							uninited = false;
							return await scope.Use<TResult, TResult>(r => Task.FromResult(r));
						}
						else
							return result;
					});

				await cb(result,ct);
			});
		}

		public static IScope<TResult> Select<TSource, TResult>(this IScope<TSource> source, Func<TSource, int, TResult> selector)
			=>Create<TResult>(
				(cb) =>
				source.Use(ictx =>
					cb(selector(ictx, 0))
					)
				);

		public static IScope<TResult> Select<TSource, TResult>(this IScope<TSource> source, Func<TSource, TResult> selector)
			=> Create<TResult>(
				(cb) =>
					source.Use(ictx =>
						cb(selector(ictx))
						)
				);

		public static IScope<TResult> SelectMany<TSource, TResult>(this IScope<TSource> source, Func<TSource, int, IScope<TResult>> selector)
			=> Create<TResult>(
				(cb) =>
					source.Use(ictx =>
						selector(ictx,0).Use(iictx=>
							cb(iictx)
						)
					)
				);
		public static IScope<TResult> SelectMany<TSource, TResult>(this IScope<TSource> source, Func<TSource, IScope<TResult>> selector)
			=> Create<TResult>(
				(cb) =>
					source.Use(ictx =>
						selector(ictx).Use(iictx =>
							 cb(iictx)
						)
					)
				);

		public static IScope<TResult> SelectMany<TSource, TCollection, TResult>(this IScope<TSource> source, Func<TSource, int, IScope<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
			=> Create<TResult>(
				(cb) =>
					source.Use(ictx =>
						collectionSelector(ictx,0).Use(iictx =>
							 cb(resultSelector(ictx,iictx))
						)
					)
				);
		public static IScope<TResult> SelectMany<TSource, TCollection, TResult>(this IScope<TSource> source, Func<TSource, IScope<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
			=> Create<TResult>(
				(cb) =>
					source.Use(ictx =>
						collectionSelector(ictx).Use(iictx =>
							  cb(resultSelector(ictx, iictx))
						)
					)
				);

		public static IScope<TResult> SelectMany<TSource, TResult>(this IScope<TSource> source, Func<TSource, int, Task<TResult>> selector)
			=> Create<TResult>(
				(cb) =>
					source.Use(async ictx =>
						await cb(await selector(ictx, 0))
					)
				);
		public static IScope<TResult> SelectMany<TSource, TResult>(this IScope<TSource> source, Func<TSource, Task<TResult>> selector)
			=> Create<TResult>(
				(cb) =>
					source.Use(async ictx =>
						await cb(await selector(ictx))
					)
				);

		public static IScope<TResult> SelectMany<TSource, TCollection, TResult>(this IScope<TSource> source, Func<TSource, int, Task<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
			=> Create<TResult>(
				(cb) =>
					source.Use(async ictx =>
						await cb(
							resultSelector(
								ictx, 
								await collectionSelector(ictx, 0)
								)
						)
					)
				);
		public static IScope<TResult> SelectMany<TSource, TCollection, TResult>(this IScope<TSource> source, Func<TSource, Task<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
			=> Create<TResult>(
				(cb) =>
					source.Use(async ictx =>
						await cb(
							resultSelector(
								ictx,
								await collectionSelector(ictx)
							)
						)
					)
				);




		public static IScope<TResult> Using<TSource, TResult>(this IScope<TSource> source, Func<TSource, TResult> selector)
			where TResult:IDisposable
			=> Create<TResult>(
				(cb) =>
					source.Use(async ictx =>
					{
						using (var re = selector(ictx))
							await cb(re);
					})
				);
		
		public static IScope<TSource> Where<TSource>(this IScope<TSource> source, Func<TSource, int, bool> predicate)
			=> Create<TSource>(
				(cb) =>
					source.Use(ictx =>
						{
							if (predicate(ictx, 0))
								return cb(ictx);
							return Task.CompletedTask;
						}
					)
			);
		public static IScope<TSource> Where<TSource>(this IScope<TSource> source, Func<TSource, bool> predicate)
			=> Create<TSource>(
				(cb) =>
					source.Use(ictx =>
					{
						if (predicate(ictx))
							return cb(ictx);
						return Task.CompletedTask;
					}
					)
			);

	}

}
