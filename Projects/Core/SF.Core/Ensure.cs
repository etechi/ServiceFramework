using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF
{
	public static class Ensure

	{
		public static void Equal<T>(
			T value,
			T expect,
			string varName
			)
			where T:IEquatable<T>
		{
			if (!value.Equals(expect))
				throw new ArgumentException(varName + "必须为" + expect);
		}
		public static void Range(
			int value,
			int min,
			int max,
			string varName
			)
		{
			if (value < min || value > max)
				throw new ArgumentOutOfRangeException(varName ?? "value", $"必须在{min}-{max}之间");
		}
		public static void Range(
			int value,
			int min,
			string varName
			)
		{
			if (value < min)
				throw new ArgumentOutOfRangeException(varName ?? "value", $"不能小于{min}");
		}
		public static void Positive(
		   int value,
		   string varName
		   )
		{
			if (value <= 0)
				throw new ArgumentOutOfRangeException(varName ?? "value", "必须为正");
		}
		public static void ZeroOrPositive(
		  int value,
		  string varName
		  )
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException(varName ?? "value", "不能为负数");
		}
		public static void NotNull(object value, string name)
		{
			if (value==null)
				throw new ArgumentNullException(name);
		}
		public static void NotEqual<T>(T value, T unexcept, string name)
			where T:IEquatable<T>
		{
			if (value.Equals(unexcept))
				throw new ArgumentException(name);
		}
		public static void NotDefault<T>(T value, string name)
			where T :IEquatable<T>
		{
			if (value.Equals(default(T)))
				throw new ArgumentNullException(name);
		}
		public static void HasContent(string str,string name)
		{
			if (string.IsNullOrWhiteSpace(str))
				throw new ArgumentException(name);
        }
		
		public static void Range(
			string strValue, 
			int minLength,
			int maxLength,
			string varName
			)
		{
			NotNull(strValue, varName);
			var l = strValue.Length;
			if(l<minLength || l>maxLength)
				throw new ArgumentOutOfRangeException(varName ?? "string",$"长度必须在{minLength}-{maxLength}之间");
        }
		public static void Range(
			string strValue,
			int minLength,
			string varName
			)
		{
			Range(strValue, minLength, int.MaxValue, varName);
		}
	}
}
