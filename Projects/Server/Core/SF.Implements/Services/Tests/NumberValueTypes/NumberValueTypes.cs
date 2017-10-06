using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using System.Linq.Expressions;

namespace SF.Services.Tests.NumberValueTypes
{
	public class NumberValueType<T> : INumberValueType<T>
	{
		public T One { get; } = (T)Convert.ChangeType(1, typeof(T));
		public T Zero { get; } = default(T);

		public T MinValue { get; } = (T)typeof(T).GetField("MinValue").GetValue(null);

		public T MaxValue { get; } = (T)typeof(T).GetField("MaxValue").GetValue(null);

		static ParameterExpression ArgX { get; } = Expression.Parameter(typeof(T), "x");
		static ParameterExpression ArgY { get; } = Expression.Parameter(typeof(T), "y");

		static Lazy<Func<T,T,T>> BinOperator(Func<Expression,Expression,Expression> Op)
			=> new Lazy<Func<T, T, T>>(() =>
			   Op(ArgX,ArgY).Compile<Func<T, T, T>>(ArgX, ArgY)
			);
		static Lazy<Func<T, T>> UnaryOperator(Func<Expression, Expression> Op)
			=> new Lazy<Func<T, T>>(() =>
			   Op(ArgX).Compile<Func<T, T>>(ArgX)
			);
		static Lazy<Func<T, T, T>> LazyAdd { get; } = BinOperator(Expression.Add);
		public T Add(T x, T y) => LazyAdd.Value(x, y);

		static Lazy<Func<T, T, T>> LazySubtract { get; } = BinOperator(Expression.Subtract);
		public T Subtract(T x, T y) => LazySubtract.Value(x, y);

		static Lazy<Func<T, T, T>> LazyMultiply { get; } = BinOperator(Expression.Multiply);
		public T Multiply(T x, T y) => LazyMultiply.Value(x, y);

		static Lazy<Func<T, T, T>> LazyDivide { get; } = BinOperator(Expression.Divide);
		public T Divide(T x, T y) => LazyDivide.Value(x, y);

		static Lazy<Func<T, T>> LazyNegate { get; } = UnaryOperator(Expression.Negate);
		public T Negate(T x) => LazyNegate.Value(x);


		public int Compare(T x, T y) => Comparer<T>.Default.Compare(x, y);

		public T ConvertFrom(int x) => (T)Convert.ChangeType(x, typeof(T));

		public static NumberValueType<T> Instance { get; } = new NumberValueType<T>();
	}
	public class NumberValueTypeProvider : INumberValueTypeProvider
	{
		public INumberValueType<T> GetNumberValueType<T>()
		{
			if (typeof(T).IsNumberLikeType())
				return NumberValueType<T>.Instance;
			throw new NotSupportedException();
		}
	}

}
