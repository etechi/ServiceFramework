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
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Reflection;
using SF.Sys.Linq.Expressions;

namespace SF.Sys.Entities.AutoEntityProvider.Internals
{
	public class PagingQueryBuilderCache: IPagingQueryBuilderCache
	{
		static MethodInfo MethodOrderByDescending { get; }
		static MethodInfo MethodOrderBy { get; }

		static PagingQueryBuilderCache()
		{
			//var methods = typeof(QueryResultBuildHelperCreator).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);

			MethodOrderByDescending = typeof(ContextQueryable).GetMethodExt(
				  "OrderByDescending",
				  typeof(IContextQueryable<>).MakeGenericType<TypeExtension.GenericTypeArgument>(),
				  typeof(Expression<>).MakeGenericType<Func<TypeExtension.GenericTypeArgument, TypeExtension.GenericTypeArgument>>()
				  ).IsNotNull();
			MethodOrderBy = typeof(ContextQueryable).GetMethodExt(
				"OrderBy",
				typeof(IContextQueryable<>).MakeGenericType<TypeExtension.GenericTypeArgument>(),
				typeof(Expression<>).MakeGenericType<Func<TypeExtension.GenericTypeArgument, TypeExtension.GenericTypeArgument>>()
				).IsNotNull();

		}


		IPagingQueryBuilder<T> BuildPagingQueryBuilder<T>()
		{
			var id = typeof(T).AllPublicInstanceProperties().Where(p => p.IsDefined(typeof(KeyAttribute))).FirstOrDefault();
			var argQueryable = Expression.Parameter(typeof(IContextQueryable<T>));
			var argDesc = Expression.Parameter(typeof(bool));
			var argEntity = Expression.Parameter(typeof(T));
			var IdMapper = Expression.Lambda(
				Expression.Property(argEntity,id),
				argEntity
				);
			var KeySelectorType = typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(typeof(T), id.PropertyType));

			var expr = Expression.Lambda<Func<IContextQueryable<T>,bool, IContextQueryable<T>>>(
				Expression.Condition(
					argDesc,
					Expression.Call(
						null,
						MethodOrderByDescending.MakeGenericMethod(
							typeof(T),
							id.PropertyType
							),
						argQueryable,
						Expression.Constant(
							IdMapper,
							KeySelectorType
							)
						),
					Expression.Call(
						null,
						MethodOrderBy.MakeGenericMethod(
							typeof(T),
							id.PropertyType
							),
						argQueryable,
						Expression.Constant(
							IdMapper,
							KeySelectorType
							)
						)
				),
				argQueryable,
				argDesc
				).Compile();

			return new PagingQueryBuilder<T>(
				id.Name, 
				b => b.Add(
					id.Name,
					expr, 
					true
					));
		}
		System.Collections.Concurrent.ConcurrentDictionary<Type, object> Builders { get; } = new System.Collections.Concurrent.ConcurrentDictionary<Type, object>();

		public IPagingQueryBuilder<T> GetBuilder<T>()
		{
			var type = typeof(T);
			if (Builders.TryGetValue(type, out var b))
				return (IPagingQueryBuilder<T>)b;
			return (IPagingQueryBuilder<T>)Builders.GetOrAdd(type, BuildPagingQueryBuilder<T>());
		}
	}
}
