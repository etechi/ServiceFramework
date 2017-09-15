using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

namespace System.Reflection
{
	public static class MethodInfoExtension
	{
		public static T CreateDelegate<T>(this MethodInfo method,object target)
		{
			return (T)(object)method.CreateDelegate(
						typeof(T),
						target
						);
		}
		public static T CreateDelegate<T>(this MethodInfo method)
		{
			return (T)(object)method.CreateDelegate(
						typeof(T)
						);
		}
	}
}
