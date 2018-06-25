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
using System.Reflection;
namespace SF.Sys.Entities
{

	public static class ContextQueryableFilters
	{
		
		public static IQueryable<O> BuildFilter<O>(
			this IQueryable<O> q,
			Expression Body,
			ParameterExpression Arg
			)
		{
			if (Body == null)
				return q;
			return q.Where(
				Expression.Lambda<Func<O, bool>>(
				   Body,
				   Arg
				));
		}
		public static IQueryable<O> BuildFilter<O, T>(
			this IQueryable<O> q,
			Expression<Func<O, T>> beginPropExpr,
			Expression<Func<O, T>> endPropExpr,
			Func<Expression, Expression, Expression> GetFilterExpression
			)
		{
			var o = Expression.Parameter(typeof(O), "o");
			var beginProp = Expression.Property(o, (System.Reflection.PropertyInfo)(beginPropExpr.Body as MemberExpression).Member);
			var endProp = Expression.Property(o, (System.Reflection.PropertyInfo)(endPropExpr.Body as MemberExpression).Member);
			var expr = GetFilterExpression(beginProp, endProp);
			if (expr == null)
				return null;
			return q.Where(
				Expression.Lambda<Func<O, bool>>(
				   expr,
				   o
				));
		}


		public static Expression GetFilterExpression<V>(
			ObjectKey<V> state,
			Expression prop
			) where V : IEquatable<V>
		{
			if (state == null)
				return null;
			if (typeof(V) == typeof(string) && ((string)(object)state.Id).IsNullOrWhiteSpace())
				return null;
			return Expression.Equal(prop, Expression.Constant(state.Id));
		}

		public static IQueryable<O> Filter<O, V>(
			   this IQueryable<O> q,
			   ObjectKey<V> state,
			   System.Linq.Expressions.Expression<Func<O, V>> propExpr
			   )
			   where O : class
				where V : IEquatable<V>
		{
			return q.BuildFilter(
				GetFilterExpression(state, propExpr.Body),
				propExpr.Parameters[0]
				);
		}

		public static Expression GetFilterExpression<V>(
			 Option<V> state,
			Expression prop
			) where V : IEquatable<V>
		{
			if (!state.HasValue)
				return null;
			return Expression.Equal(prop, Expression.Constant(state.Value));
		}
		public static IQueryable<O> Filter<O,V>(
			   this IQueryable<O> q,
			   Option<V> state,
			   System.Linq.Expressions.Expression<Func<O, V>> propExpr
			   )
			   where O : class
				where V: IEquatable<V>
		{
			return q.BuildFilter(
				GetFilterExpression(state, propExpr.Body),
				propExpr.Parameters[0]
				);
		}

		public static Expression GetFilterExpression(
			QueryableBoolean? state,
			Expression prop
			) 
		{
			if (!state.HasValue)
				return null;
			return Expression.Equal(prop, Expression.Constant(state.Value == QueryableBoolean.True));
		}
		public static IQueryable<O> Filter<O>(
             this IQueryable<O> q,
             QueryableBoolean? state,
             System.Linq.Expressions.Expression<Func<O, bool>> propExpr
             )
             where O : class
        {
			return q.BuildFilter(
				 GetFilterExpression(state, propExpr.Body),
				 propExpr.Parameters[0]
				 );
		}
		public static Expression GetFilterExpression(
			EntityLogicState? state,
			Expression prop
			)
		{
			if (!state.HasValue)
				return Expression.NotEqual(prop, Expression.Constant(EntityLogicState.Deleted));
			return Expression.Equal(prop, Expression.Constant(state.Value));
		}
		public static IQueryable<O> Filter<O>(
             this IQueryable<O> q,
             EntityLogicState? state,
             System.Linq.Expressions.Expression<Func<O, EntityLogicState>> propExpr
             )
             where O : class
        {
			return q.BuildFilter(
				  GetFilterExpression(state, propExpr.Body),
				  propExpr.Parameters[0]
				  );
		}
		public static Expression GetStringFilterExpression(
			string value,
			Expression prop
			)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;
			return Expression.Equal(prop, Expression.Constant(value.Trim()));
		}
		public static IQueryable<O> Filter<O>(
           this IQueryable<O> q,
           string value,
           System.Linq.Expressions.Expression<Func<O, string>> propExpr
           )
           where O : class
        {
			return q.BuildFilter(
				  GetStringFilterExpression(value, propExpr.Body),
				  propExpr.Parameters[0]
				  );
		}
		public static Expression GetFilterExpression<V>(
			V? state,
			Expression prop
			) where V : struct
		{
			if (!state.HasValue)
				return null;
			return Expression.Equal(prop, Expression.Constant(state.Value));
		}

		public static IQueryable<O> Filter<O,T>(
            this IQueryable<O> q,
            T? value,
            System.Linq.Expressions.Expression<Func<O, T>> propExpr
            )
            where O : class
            where T:struct
        {
			return q.BuildFilter(
				   GetFilterExpression(value, propExpr.Body),
				   propExpr.Parameters[0]
				   );
		}
		
		class NullableProps<T>
            where T:struct
        {
            public static PropertyInfo HasValue { get; } = typeof(T?).GetProperty("HasValue");
            public static PropertyInfo Value { get; } = typeof(T?).GetProperty("Value");
        }
		public static Expression GetNullableFilterExpression<T>(
			T? value,
			Expression prop,
			bool UseDefaultValueForNoValue = true
			) where T : struct
		{
			if (!value.HasValue)
				return null;
			if (UseDefaultValueForNoValue && 
				value.HasValue && 
				EqualityComparer<T>.Default.Equals(value.Value, default(T))
				)
			{
				return Expression.Not(Expression.Property(prop, NullableProps<T>.HasValue));
			}
			return Expression.AndAlso(
					Expression.Property(prop, NullableProps<T>.HasValue),
					Expression.Equal(
						Expression.Property(prop, NullableProps<T>.Value),
						Expression.Constant(value.Value)
						)
					);
		}

		public static IQueryable<O> Filter<O, T>(
            this IQueryable<O> q,
            T? value,
            System.Linq.Expressions.Expression<Func<O, T?>> propExpr,
			bool UseDefaultValueForNoValue=true
			)
            where O : class
            where T : struct
        {
			return q.BuildFilter(
				   GetNullableFilterExpression(value, propExpr.Body,UseDefaultValueForNoValue),
				   propExpr.Parameters[0]
				   );
		}

        static MethodInfo StringContains { get; } = typeof(string)
			.GetMethod("Contains", BindingFlags.Instance | BindingFlags.Public,null,new[] { typeof(string) },null)
			.IsNotNull();

		public static Expression GetContainFilterExpression(
			string value,
			Expression prop
			)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;
			return Expression.Call(
					   prop,
					   StringContains,
					   Expression.Constant(value.Trim())
					   );
		}

		public static IQueryable<O> FilterContains<O>(
            this IQueryable<O> q,
            string value,
            System.Linq.Expressions.Expression<Func<O, string>> propExpr
            )
            where O : class
        {
			return q.BuildFilter(
					GetContainFilterExpression(value, propExpr.Body),
					propExpr.Parameters[0]
					);
		}



		public static Expression GetFilterExpression<T>(
			QueryRange<T> range,
			Expression prop,
			 T? DefaultBeginValue = null
			) where T : struct, IComparable<T>
		{
			if (range == null || !range.Begin.HasValue && !range.End.HasValue)
			{
				if (DefaultBeginValue == null)
					return null;
				return Expression.LessThanOrEqual(
						Expression.Constant(DefaultBeginValue.Value),
						prop
						);
			}
			else if (range.Begin == null && range.End != null)
				return Expression.LessThanOrEqual(prop, Expression.Constant(range.End.Value));
			else if (range.Begin != null && range.End == null)
				return Expression.LessThanOrEqual(Expression.Constant(range.Begin.Value), prop);
			else
				return Expression.AndAlso(
					Expression.LessThanOrEqual(Expression.Constant(range.Begin.Value), prop),
					Expression.LessThanOrEqual(prop, Expression.Constant(range.End.Value))
					);
		}

		public static IQueryable<O> Filter<O,T>(
            this IQueryable<O> q,
            QueryRange<T> range,
            System.Linq.Expressions.Expression<Func<O, T>> propExpr,
            T? DefaultBeginValue = null
            )
            where O : class
            where T:struct,IComparable<T>
        {
			return q.BuildFilter(
					GetFilterExpression(range, propExpr.Body,DefaultBeginValue),
					propExpr.Parameters[0]
					);
		}
		public static Expression GetNullableRangeFilterExpression<T>(
			NullableQueryRange<T> range,
			Expression prop
			) where T : struct, IComparable<T>
		{
			if (range == null || !range.NotNull && !range.Begin.HasValue && !range.End.HasValue)
				return null;
			else if (range.NotNull && !range.Begin.HasValue && !range.End.HasValue)
			{
				return Expression.Property(prop, NullableProps<T>.HasValue);
			}
			else if (range.Begin == null && range.End != null)
				return Expression.AndAlso(
					Expression.Property(prop, NullableProps<T>.HasValue),
					Expression.LessThanOrEqual(
						Expression.Property(prop, NullableProps<T>.Value),
						Expression.Constant(range.End.Value)
						)
					);
			else if (range.Begin != null && range.End == null)
				return Expression.AndAlso(
					 Expression.Property(prop, NullableProps<T>.HasValue),
					 Expression.LessThanOrEqual(
						 Expression.Constant(range.Begin.Value),
						 Expression.Property(prop, NullableProps<T>.Value)
						 )
					 );
			else
				return Expression.AndAlso(
				   Expression.Property(prop, NullableProps<T>.HasValue),
					Expression.AndAlso(
						Expression.LessThanOrEqual(
							 Expression.Constant(range.Begin.Value),
							 Expression.Property(prop, NullableProps<T>.Value)
						 ),
						 Expression.LessThanOrEqual(
							 Expression.Property(prop, NullableProps<T>.Value),
							 Expression.Constant(range.End.Value)
							 )
						 )
					 );
		}

		public static IQueryable<O> Filter<O,T>(
            this IQueryable<O> q,
            NullableQueryRange<T> range,
            System.Linq.Expressions.Expression<Func<O, T?>> propExpr
            )
            where O : class
            where T : struct,IComparable<T>
        {
			return q.BuildFilter(
					  GetNullableRangeFilterExpression(range, propExpr.Body),
					  propExpr.Parameters[0]
					  );
		}

		public static Expression GetFilterExpression<T>(
			QueryRange<T> range,
			Expression beginProp,
			Expression endProp,
			T? DefaultBeginValue = null
			) where T : struct, IComparable<T>
		{
			if (range == null || !range.Begin.HasValue && !range.End.HasValue)
			{
				if (DefaultBeginValue == null)
					return null;
				return Expression.LessThanOrEqual(
						Expression.Constant(DefaultBeginValue.Value),
						beginProp
						);
			}
			else if (range.Begin == null && range.End != null)
				return Expression.LessThanOrEqual(endProp, Expression.Constant(range.End.Value));
			else if (range.Begin != null && range.End == null)
				return Expression.LessThanOrEqual(Expression.Constant(range.Begin.Value), beginProp);
			else
				return Expression.AndAlso(
					Expression.LessThanOrEqual(Expression.Constant(range.Begin.Value), beginProp),
					Expression.LessThanOrEqual(endProp, Expression.Constant(range.End.Value))
					);

		}
	
		


		public static IQueryable<O> Filter<O,T>(
            this IQueryable<O> q,
            QueryRange<T> range,
            System.Linq.Expressions.Expression<Func<O, T>> beginPropExpr,
            System.Linq.Expressions.Expression<Func<O, T>> endPropExpr,
            T? DefaultBeginValue = null
            )
            where O : class
            where T:struct ,IComparable<T>
        {
			return q.BuildFilter(
				beginPropExpr,
				endPropExpr,
				(b, e) => GetFilterExpression(range, b, e, DefaultBeginValue)
				);
        }

		public static Expression GetNullableRangeFilterExpression<T>(
			QueryRange<T> range,
			Expression beginProp,
			Expression endProp
			) where T : struct, IComparable<T>
		{
			if (range == null || !range.Begin.HasValue && !range.End.HasValue)
				return null;
			else if (range.Begin == null && range.End != null)
				return Expression.AndAlso(
					Expression.Property(endProp, NullableProps<T>.HasValue),
					Expression.LessThanOrEqual(
						Expression.Property(endProp, NullableProps<T>.Value),
						Expression.Constant(range.End.Value)
						)
					);
			else if (range.Begin != null && range.End == null)
				return Expression.AndAlso(
					Expression.Property(beginProp, NullableProps<T>.HasValue),
					Expression.LessThanOrEqual(
						Expression.Constant(range.Begin.Value),
						Expression.Property(beginProp, NullableProps<T>.Value)
						)
					);
			else
				return Expression.AndAlso(
					Expression.AndAlso(
						Expression.Property(beginProp, NullableProps<T>.HasValue),
							Expression.LessThanOrEqual(
							Expression.Constant(range.Begin.Value),
							Expression.Property(beginProp, NullableProps<T>.Value)
							)
						),
					Expression.AndAlso(
						Expression.Property(endProp, NullableProps<T>.HasValue),
						Expression.LessThanOrEqual(
							Expression.Property(endProp, NullableProps<T>.Value),
							Expression.Constant(range.End.Value)
							)
						)
					);

		}


		public static IQueryable<O> Filter<O,T>(
           this IQueryable<O> q,
           QueryRange<T> range,
           System.Linq.Expressions.Expression<Func<O, T?>> beginPropExpr,
           System.Linq.Expressions.Expression<Func<O, T?>> endPropExpr
           )
           where O : class
            where T:struct,IComparable<T>
        {

			return q.BuildFilter(
				 beginPropExpr,
				 endPropExpr,
				 (b, e) => GetNullableRangeFilterExpression(range, b, e)
				 );
		}


        public static IQueryable<O> Filter<O,T>(
              this IQueryable<O> q,
              QueryRange<T> range,
              System.Linq.Expressions.Expression<Func<O, T>> propExpr
              )
              where O : class
        where T : struct, IComparable<T>
        {
            return q.Filter(range, propExpr, null);
        }
        public static IQueryable<O> Filter<O>(
            this IQueryable<O> q,
            QueryRange<DateTime> range,
            System.Linq.Expressions.Expression<Func<O, DateTime>> propExpr
            )
            where O : class
        {
            return q.Filter(
                range, 
                propExpr, 
                DateTime.Now.Date.AddDays(-31)
                );
        }
        public static IQueryable<O> Filter<O>(
            this IQueryable<O> q,
            NullableQueryRange<DateTime> range,
            System.Linq.Expressions.Expression<Func<O, DateTime?>> propExpr
            )
            where O : class
        {
            return q.Filter<O,DateTime>(
                range, 
                propExpr 
                );
        }
		
        public static IQueryable<O> Filter<O>(
            this IQueryable<O> q,
            QueryRange<DateTime> range,
            System.Linq.Expressions.Expression<Func<O, DateTime>> beginPropExpr,
            System.Linq.Expressions.Expression<Func<O, DateTime>> endPropExpr
            )
            where O : class
        {
            return q.Filter(
                range, 
                beginPropExpr, 
                endPropExpr,
                DateTime.Now.Date.AddDays(-31)
                );
        }


        public static IQueryable<O> Filter<O>(
           this IQueryable<O> q,
           QueryRange<DateTime> range,
           System.Linq.Expressions.Expression<Func<O, DateTime?>> beginPropExpr,
           System.Linq.Expressions.Expression<Func<O, DateTime?>> endPropExpr
           )
           where O : class
        {

            return q.Filter<O,DateTime>(
                range, 
                beginPropExpr, 
                endPropExpr
                );
        }
    }
}
