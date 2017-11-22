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
using System.Collections.Generic;
using System.Linq;
using SF.Sys.Reflection;
using SF.Sys.Linq.Expressions;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.QueryFilterProviders
{
	public class PropQueryFilterProvider : IQueryFilterProvider
	{
		class PropQueryFilter<TDataModel, TQueryArgument> : IQueryFilter<TDataModel, TQueryArgument> 
		{
			public int Priority => 0;
			Lazy<Func<TQueryArgument, Expression<Func<TDataModel, bool>>>> GetFilter{get;}
			public PropQueryFilter(Lazy<Func<TQueryArgument, Expression<Func<TDataModel, bool>>>> GetFilter)
			{
				this.GetFilter = GetFilter;
			}
			public IContextQueryable<TDataModel> Filter(IContextQueryable<TDataModel> Query, IEntityServiceContext ServiceContext, TQueryArgument Arg)
			{
				var cond = GetFilter.Value(Arg); 
				if (cond == null) return Query;
				return Query.Where(cond);
			}
		}

		public IPropertyQueryFilterProvider[] PropertyQueryFilterProviders { get; }
		public PropQueryFilterProvider(IEnumerable<IPropertyQueryFilterProvider> PropertyQueryFilterProviders)
		{
			this.PropertyQueryFilterProviders = PropertyQueryFilterProviders.OrderBy(p=>p.Priority).ToArray();
		}
		static MethodInfo MethodLambda { get; } = typeof(Expression).GetMethods(
			nameof(Expression.Lambda),
			BindingFlags.Public | BindingFlags.Static
			).Single(m =>
			{
				if (!m.IsGenericMethodDefinition)
					return false;
				var ps = m.GetParameters();
				return ps.Length == 2 && ps[0].ParameterType == typeof(Expression) && ps[1].ParameterType == typeof(IEnumerable<ParameterExpression>);
			});
		static MethodInfo MethodAndAlso { get; } = typeof(Expression).GetMethod(nameof(Expression.AndAlso), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(Expression), typeof(Expression) }, null);
		static Expression<Func<TModel, bool>> CreateLambda<TModel>(
			ParameterExpression argModel, 
			Expression[] Exprs
			)
		{
			var es = Exprs.Where(x => x != null).ToArray();
			if (es.Length == 0)
				return null;
			Expression expr;
			if (es.Length == 1)
				expr = es[0];
			else
			{
				Array.Reverse(es);
				expr = es.Aggregate((x, y) => Expression.AndAlso(y, x));
			}
			return Expression.Lambda<Func<TModel,bool>>(
				expr,
				argModel
				);
		}
		public IQueryFilter<TDataModel, TQueryArgument> GetFilter<TDataModel, TQueryArgument>()
		{
			var filters = (
				from prop in typeof(TQueryArgument).AllPublicInstanceProperties()
				let filter=PropertyQueryFilterProviders
						.Select(p => p.GetFilter<TDataModel, TQueryArgument>(prop))
						.Where(f => f != null)
						.FirstOrDefault()
				where filter!=null
				orderby filter.Priority
				select (prop: prop, filter: filter)
				).ToArray();
			if (filters.Length == 0)
				return null;
		
			return new PropQueryFilter<TDataModel, TQueryArgument>(
				new Lazy<Func<TQueryArgument, Expression<Func<TDataModel, bool>>>>(() =>
				{
					var argQuery = Expression.Parameter(typeof(TQueryArgument));
					var argModel = Expression.Parameter(typeof(TDataModel));
					var conds = filters.Select(p =>
								  Expression.Constant(
									  p.filter,
									  typeof(IPropertyQueryFilter<>).MakeGenericType(p.prop.PropertyType)
									  ).CallMethod(
										  nameof(IPropertyQueryFilter<int>.GetFilterExpression),
										  Expression.Constant(argModel),
										  argQuery.GetMember(p.prop)
									  )
								);
					var body = Expression.Call(
						typeof(PropQueryFilterProvider).GetMethodExt(
							nameof(CreateLambda),
							BindingFlags.Static | BindingFlags.NonPublic,
							new[] {typeof(ParameterExpression), typeof(Expression[]) }
							).MakeGenericMethod(typeof(TDataModel)),
						Expression.Constant(argModel),
						Expression.NewArrayInit(
							typeof(Expression),
							conds
						)
						);
					var getfilter = Expression.Lambda<Func<TQueryArgument, Expression<Func<TDataModel, bool>>>>(
							body,
							argQuery
						).Compile(); 

					return getfilter;
				})
			);
		}
	}
}
