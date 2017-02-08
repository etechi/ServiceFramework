using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF
{
	public static class UIEnsure
	{
		public static void Error(string message)
		{
			throw new UserInputException(message);
		}
		public static void Range<T>(
			T value,
			T min,
			T max,
			string message
			) where T:IComparable<T>
		{
			if (value.CompareTo(min)<0 || value.CompareTo(max)>0)
				Error(string.Format(message,min,max));
		}
		public static void Min<T>(
            T value,
            T min,
			string message
			) where T : IComparable<T>
        {
			if (value.CompareTo(min)<0)
				Error(string.Format(message,min));
		}
		public static void Max<T>(
            T value,
            T max,
			string message
            ) where T : IComparable<T>
        {
            if (value.CompareTo(max) > 0)
                Error(string.Format(message, max));
		}
		public static void Positive(
		   int value,
		   string message
		   )
		{
			if (value <= 0)
				Error(message);
		}
		public static void ZeroOrPositive(
		  int value,
		  string message
		  )
		{
			if (value < 0)
				Error(message);
		}
		public static void NotNull(object value, string message)
		{
			if (value==null)
				Error(message);
		}
		public static void NotDefault<T>(T value, string message)
			where T :IEquatable<T>
		{
			if (value.Equals(default(T)))
				Error(message);
		}
		public static void HasContent(string str,string name)
		{
			if (string.IsNullOrWhiteSpace(str))
				Error(name);
        }
		
		public static void Range(
			string strValue, 
			int minLength,
			int maxLength,
			string message
			)
		{
			NotNull(strValue, message);
			var l = strValue.Length;
			if (l < minLength || l > maxLength)
				Error(string.Format(message, minLength, maxLength));
        }
		public static void MinLength(
			string strValue,
			int minLength,
			string message
			)
		{
			Range(strValue, minLength, int.MaxValue, message);
		}
	}
}
