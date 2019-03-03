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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys
{
	public static class ObjectExtension
	{
		public static bool IsDefault<T>(this T value)
		{
			return EqualityComparer<T>.Default.Equals(value, default(T));
		}

		static MethodInfo IsDefaultMethodInfo { get; } = typeof(ObjectExtension).GetMethods(
			BindingFlags.Static |
			BindingFlags.InvokeMethod |
			BindingFlags.Public)
			.Where(m => 
				m.Name == "IsDefault" && 
				m.IsGenericMethodDefinition && 
				m.GetParameters().Length == 1)
			.Single();
		static ConcurrentDictionary<Type, Func<object, bool>> Dicts { get; } = new ConcurrentDictionary<Type, Func<object, bool>>();
		public static bool IsDefaultInstance(this Type Type,object value)
		{
			if (Type.IsValueType)
			{
				if (value==null || value.GetType() != Type)
					throw new ArgumentException();
				if (!Dicts.TryGetValue(Type, out var f))
				{
					var arg = Expression.Parameter(typeof(object));
					f = Dicts.GetOrAdd(
						Type,
						Expression.Lambda<Func<object, bool>>(
							Expression.Call(
								IsDefaultMethodInfo.MakeGenericMethod(Type),
								Expression.Unbox(arg, Type)
							),
							arg
						).Compile()
						);
				}
				return f(value);
			}
			else
				return value == null;
		}
	}
}
