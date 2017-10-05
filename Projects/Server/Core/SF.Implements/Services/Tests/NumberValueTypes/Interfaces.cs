using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;

namespace SF.Services.Tests.NumberValueTypes
{
	public interface IValueRange<T>
	{
		T MinValue { get; }
		T MaxValue { get; }
	}
	public interface INumberValueType<T> : IComparer<T>, IValueRange<T>
	{
		T One { get; }
		T Zero { get; }
		T Add(T x, T y);
		T Subtract(T x, T y);
		T Multiply(T x, T y);
		T Divide(T x, T y);
		T ConvertFrom(int x);
		T Negate(T x);
	}
	public static class ValueTypeExtension
	{
		public static T Max<T>(this INumberValueType<T> Type, T X, T Y)
			=> Type.Compare(X, Y) >= 0 ? X : Y;

		public static T Min<T>(this INumberValueType<T> Type, T X, T Y)
			=> Type.Compare(X, Y) <= 0 ? X : Y;
	}
	public interface INumberValueTypeProvider
	{
		INumberValueType<T> GetNumberValueType<T>();
	}
}
