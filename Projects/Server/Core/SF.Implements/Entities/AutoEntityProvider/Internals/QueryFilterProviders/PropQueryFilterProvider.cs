using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;
using System.Reflection.Emit;
using SF.Core.ServiceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Linq.TypeExpressions;

namespace SF.Entities.AutoEntityProvider.Internals.QueryFilterProviders
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
			public IContextQueryable<TDataModel> Filter(IContextQueryable<TDataModel> Query, TQueryArgument Arg)
			{
				var cond = GetFilter.Value(Arg);
				if (cond == null) return Query;
				return Query.Where(cond);
			}
		}

		public IPropertyQueryFilterProvider[] PropertyQueryFilterProviders { get; }
		public PropQueryFilterProvider(IEnumerable<IPropertyQueryFilterProvider> PropertyQueryFilterProviders)
		{
			this.PropertyQueryFilterProviders = PropertyQueryFilterProviders.ToArray();
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
		static Expression CombineConditions(Expression[] Exprs)
		{
			var es = Exprs.Where(x => x != null).ToArray();
			if (es.Length == 0)
				return null;
			if (es.Length == 1)
				return es[0];
			Array.Reverse(es);
			return es.Aggregate((x, y) => Expression.AndAlso(y, x));
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
						typeof(PropQueryFilterProvider).GetMethod(
							nameof(CombineConditions),
							BindingFlags.Static | BindingFlags.NonPublic,
							null,
							new[] { typeof(Expression[]) },
							null
							),
						Expression.NewArrayInit(
							typeof(Expression),
							conds
						)
						);
					var getfilter = Expression.Lambda<Func<TQueryArgument, Expression<Func<TDataModel, bool>>>>(
						Expression.Call(
							null,
							MethodLambda.MakeGenericMethod(typeof(Func<TDataModel, bool>)),
							body,
							Expression.NewArrayInit(
								typeof(ParameterExpression),
								Expression.Constant(argModel)
								)
							),
							argQuery
						).Compile();

					return getfilter;
				})
			);
		}
	}
}
