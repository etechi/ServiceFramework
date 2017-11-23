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


using System.Collections.Generic;

namespace SF.Sys.Tests.NumberValueTypes
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
