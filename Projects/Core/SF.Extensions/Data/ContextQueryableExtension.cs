using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SF.Data
{

    public static class ContextQueryableExtension
	{
		public static IContextQueryable<O> Filter<O,V>(
			   this IContextQueryable<O> q,
			   Option<V> state,
			   System.Linq.Expressions.Expression<Func<O, V>> propExpr
			   )
			   where O : class
				where V: IEquatable<V>
		{
			if (!state.HasValue)
				return q;
			var o = propExpr.Parameters[0];
			var prop = propExpr.Body;
			return q.Where(
				Expression.Lambda<Func<O, bool>>(
				   Expression.Equal(prop, Expression.Constant(state.Value)),
				   o
				));
		}
		public static IContextQueryable<O> Filter<O>(
             this IContextQueryable<O> q,
             QueryableBoolean? state,
             System.Linq.Expressions.Expression<Func<O, bool>> propExpr
             )
             where O : class
        {
            if (!state.HasValue)
                return q;
            var o = propExpr.Parameters[0];
            var prop = propExpr.Body;
            return q.Where(
                Expression.Lambda<Func<O, bool>>(
                   Expression.Equal(prop, Expression.Constant(state.Value==QueryableBoolean.True)),
                   o
                ));
        }

        public static IContextQueryable<O> Filter<O>(
             this IContextQueryable<O> q,
             LogicObjectState? state,
             System.Linq.Expressions.Expression<Func<O, LogicObjectState>> propExpr
             )
             where O : class
        {
            var o = propExpr.Parameters[0];
            var prop = propExpr.Body;
            if (state == null)
                return q.Where(Expression.Lambda<Func<O, bool>>(
                   Expression.NotEqual(prop, Expression.Constant(LogicObjectState.Deleted)),
                   o
                   ));
            return q.Where(
                Expression.Lambda<Func<O, bool>>(
                   Expression.Equal(prop, Expression.Constant(state.Value)),
                   o
                ));
        }
        public static IContextQueryable<O> Filter<O>(
           this IContextQueryable<O> q,
           string value,
           System.Linq.Expressions.Expression<Func<O, string>> propExpr
           )
           where O : class
        {
            var o = propExpr.Parameters[0];
            var prop = propExpr.Body;
            if (string.IsNullOrWhiteSpace(value))
                return q;
            return q.Where(
                Expression.Lambda<Func<O, bool>>(
                   Expression.Equal(prop, Expression.Constant(value.Trim())),
                   o
                ));
        }
        public static IContextQueryable<O> Filter<O,T>(
            this IContextQueryable<O> q,
            T? value,
            System.Linq.Expressions.Expression<Func<O, T>> propExpr
            )
            where O : class
            where T:struct
        {
            var o = propExpr.Parameters[0];
            var prop = propExpr.Body;
            if (value == null)
                return q;
            return q.Where(
                Expression.Lambda<Func<O, bool>>(
                   Expression.Equal(prop, Expression.Constant(value.Value)),
                   o
                ));
        }
        class NullableProps<T>
            where T:struct
        {
            public static PropertyInfo HasValue { get; } = typeof(T?).GetProperty("HasValue");
            public static PropertyInfo Value { get; } = typeof(T?).GetProperty("Value");
        }


        public static IContextQueryable<O> Filter<O, T>(
            this IContextQueryable<O> q,
            T? value,
            System.Linq.Expressions.Expression<Func<O, T?>> propExpr
            )
            where O : class
            where T : struct
        {
            var o = propExpr.Parameters[0];
            var prop = propExpr.Body;
            if (value == null)
                return q;
            return q.Where(
                Expression.Lambda<Func<O, bool>>(
                    Expression.AndAlso(
                        Expression.Property(prop,NullableProps<T>.HasValue),
                        Expression.Equal(
                            Expression.Property(prop, NullableProps<T>.Value), 
                            Expression.Constant(value.Value)
                            )
                    ),
                    o
                ));
        }

        readonly static MethodInfo StringContains = typeof(string).GetMethod("Contains", BindingFlags.Instance | BindingFlags.Public );
        public static IContextQueryable<O> FilterContains<O>(
            this IContextQueryable<O> q,
            string value,
            System.Linq.Expressions.Expression<Func<O, string>> propExpr
            )
            where O : class
        {
            var o = propExpr.Parameters[0];
            var prop = propExpr.Body;
            if (string.IsNullOrWhiteSpace(value))
                return q;
            return q.Where(
                Expression.Lambda<Func<O, bool>>(
                   Expression.Call(
                       prop,
                       StringContains,
                       Expression.Constant(value.Trim())
                       ),
                   o
                ));
        }





        public static IContextQueryable<O> Filter<O,T>(
            this IContextQueryable<O> q,
            QueryRange<T> range,
            System.Linq.Expressions.Expression<Func<O, T>> propExpr,
            T? DefaultBeginValue = null
            )
            where O : class
            where T:struct,IComparable<T>
        {
            var o = propExpr.Parameters[0];
            var prop = propExpr.Body;

            Expression expr;
            if (range == null || !range.Begin.HasValue && !range.End.HasValue)
            {
                if (DefaultBeginValue == null)
                    return q;
                expr = Expression.LessThanOrEqual(
                        Expression.Constant(DefaultBeginValue.Value),
                        prop
                        );
            }
            else if (range.Begin == null && range.End != null)
                expr = Expression.LessThanOrEqual(prop, Expression.Constant(range.End.Value));
            else if (range.Begin != null && range.End == null)
                expr = Expression.LessThanOrEqual(Expression.Constant(range.Begin.Value), prop);
            else
                expr = Expression.AndAlso(
                    Expression.LessThanOrEqual(Expression.Constant(range.Begin.Value), prop),
                    Expression.LessThanOrEqual(prop, Expression.Constant(range.End.Value))
                    );

            return q.Where(
                   Expression.Lambda<Func<O, bool>>(
                   expr,
                   o
                   )
                   );
        }
        public static IContextQueryable<O> Filter<O,T>(
            this IContextQueryable<O> q,
            NullableQueryRange<T> range,
            System.Linq.Expressions.Expression<Func<O, T?>> propExpr
            )
            where O : class
            where T : struct,IComparable<T>
        {
            var o = propExpr.Parameters[0];
            var prop = propExpr.Body;

            Expression expr;
            if (range == null || !range.NotNull && !range.Begin.HasValue && !range.End.HasValue)
            {
                return q;
            }
            else if(range.NotNull && !range.Begin.HasValue && !range.End.HasValue)
            {
                expr = Expression.Property(prop, NullableProps<T>.HasValue);
            }
            else if (range.Begin == null && range.End != null)
                expr = Expression.AndAlso(
                    Expression.Property(prop, NullableProps<T>.HasValue),
                    Expression.LessThanOrEqual(
                        Expression.Property(prop, NullableProps<T>.Value),
                        Expression.Constant(range.End.Value)
                        )
                    );
            else if (range.Begin != null && range.End == null)
                expr = Expression.AndAlso(
                     Expression.Property(prop, NullableProps<T>.HasValue),
                     Expression.LessThanOrEqual(
                         Expression.Constant(range.Begin.Value),
                         Expression.Property(prop, NullableProps<T>.Value)
                         )
                     );
            else
                expr = Expression.AndAlso(
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

            return q.Where(
                   Expression.Lambda<Func<O, bool>>(
                   expr,
                   o
                   )
                   );
        }
        public static IContextQueryable<O> Filter<O,T>(
            this IContextQueryable<O> q,
            QueryRange<T> range,
            System.Linq.Expressions.Expression<Func<O, T>> beginPropExpr,
            System.Linq.Expressions.Expression<Func<O, T>> endPropExpr,
            T? DefaultBeginValue = null
            )
            where O : class
            where T:struct ,IComparable<T>
        {
            var o = Expression.Parameter(typeof(O), "o");
            var beginProp = Expression.Property(o, (System.Reflection.PropertyInfo)(beginPropExpr.Body as MemberExpression).Member);
            var endProp = Expression.Property(o, (System.Reflection.PropertyInfo)(endPropExpr.Body as MemberExpression).Member);

            Expression expr;
            if (range == null || !range.Begin.HasValue && !range.End.HasValue)
            {
                if (DefaultBeginValue == null)
                    return q;
                expr = Expression.LessThanOrEqual(
                        Expression.Constant(DefaultBeginValue.Value),
                        beginProp
                        );
            }
            else if (range.Begin == null && range.End != null)
                expr = Expression.LessThanOrEqual(endProp, Expression.Constant(range.End.Value));
            else if (range.Begin != null && range.End == null)
                expr = Expression.LessThanOrEqual(Expression.Constant(range.Begin.Value), beginProp);
            else
                expr = Expression.AndAlso(
                    Expression.LessThanOrEqual(Expression.Constant(range.Begin.Value), beginProp),
                    Expression.LessThanOrEqual(endProp, Expression.Constant(range.End.Value))
                    );

            return q.Where(
                   Expression.Lambda<Func<O, bool>>(
                   expr,
                   o
                   )
                   );
        }

        public static IContextQueryable<O> Filter<O,T>(
           this IContextQueryable<O> q,
           QueryRange<T> range,
           System.Linq.Expressions.Expression<Func<O, T?>> beginPropExpr,
           System.Linq.Expressions.Expression<Func<O, T?>> endPropExpr
           )
           where O : class
            where T:struct,IComparable<T>
        {

            var o = Expression.Parameter(typeof(O), "o");
            var beginProp = Expression.Property(o, (System.Reflection.PropertyInfo)(beginPropExpr.Body as MemberExpression).Member);
            var endProp = Expression.Property(o, (System.Reflection.PropertyInfo)(endPropExpr.Body as MemberExpression).Member);

            Expression expr;
            if (range == null || !range.Begin.HasValue && !range.End.HasValue)
            {
                return q;
            }
            else if (range.Begin == null && range.End != null)
                expr = Expression.AndAlso(
                    Expression.Property(endProp, NullableProps<T>.HasValue),
                    Expression.LessThanOrEqual(
                        Expression.Property(endProp, NullableProps<T>.Value),
                        Expression.Constant(range.End.Value)
                        )
                    );
            else if (range.Begin != null && range.End == null)
                expr = Expression.AndAlso(
                    Expression.Property(beginProp, NullableProps<T>.HasValue),
                    Expression.LessThanOrEqual(
                        Expression.Constant(range.Begin.Value),
                        Expression.Property(beginProp, NullableProps<T>.Value)
                        )
                    );
            else
                expr = Expression.AndAlso(
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

            return q.Where(
                   Expression.Lambda<Func<O, bool>>(
                   expr,
                   o
                   )
                   );
        }


        public static IContextQueryable<O> Filter<O,T>(
              this IContextQueryable<O> q,
              QueryRange<T> range,
              System.Linq.Expressions.Expression<Func<O, T>> propExpr
              )
              where O : class
        where T : struct, IComparable<T>
        {
            return q.Filter(range, propExpr, null);
        }
        public static IContextQueryable<O> Filter<O>(
            this IContextQueryable<O> q,
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
        public static IContextQueryable<O> Filter<O>(
            this IContextQueryable<O> q,
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
        public static IContextQueryable<O> Filter<O>(
            this IContextQueryable<O> q,
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


        public static IContextQueryable<O> Filter<O>(
           this IContextQueryable<O> q,
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
