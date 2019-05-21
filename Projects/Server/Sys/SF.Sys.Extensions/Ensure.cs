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
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys
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
		public static void Positive(
		  long value,
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
		
		public static T Assert<T>(this T value, Func<T,bool> Test, Func<T,string> Message) 
		{
			if (!Test(value))
				throw new InvalidOperationException(Message(value));
			return value;
		}
        public static void Assert(this bool value, string Message)
        {
            if (!value)
                throw new InvalidOperationException(Message);
        }
        public static T Error<T>(this T value, Func<T, bool> Test, Func<T, string> Message)
		{
			if (Test(value))
				throw new InvalidOperationException(Message(value));
			return value;
		}
		public static T IsNotNull<T>(this T value, Func< string> Message=null) where T : class
		{
			if (value == null)
			{
				if(Message==null)
					throw new InvalidOperationException($"监测到异常的空对象:{typeof(T)}");
				else
					throw new InvalidOperationException(Message());
			}
			return value;
		}
        public static async Task<T> IsNotNull<T>(this Task<T> value, Func<string> Message = null) where T : class
        {
            var re = await value;
            return re.IsNotNull(Message);
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
