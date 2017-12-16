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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using SF.Sys.Reflection;
using SF.Sys.Linq.Expressions;
using SF.Sys.Linq;

namespace SF.Sys
{
	public  static partial class Poco
	{
		static class TableSerializationHelper<T>
		{
			public static PropertyInfo[] Props { get; } =
				typeof(T).AllPublicInstanceProperties()
				.Where(p =>
					p.PropertyType.IsConstType() ||
					(p.PropertyType.GetGenericArgumentTypeAsNullable()?.IsConstType() ?? false)
				)
				.ToArray();
			static Expression Arg { get; } = Expression.Parameter(typeof(T), "o");
			static Expression Value { get; } = Expression.Parameter(typeof(string), "v");

			public static Dictionary<string, Lazy<Func<T, string>>> Getters { get; } = Props.ToDictionary(
				p => p.Name,
				p => new Lazy<Func<T, string>>(() =>
				  {
					  var func = Expression.Convert(
						  Expression.Call(
							  Arg,
							  p.GetGetMethod()
							  ),
						  typeof(object)
						  ).Compile<Func<T, object>>(Arg);
					  return o => func(o)?.ToString() ?? "";
				  })
				);

			public static Dictionary<string, Lazy<Action<T, string>>> Setters { get; } = Props.ToDictionary(
				p => p.Name,
				p => new Lazy<Action<T, string>>(() =>
				{
					var va = Expression.Parameter(typeof(object), "v");
					var func = Expression.Call(
						Arg,
						p.GetSetMethod(),
						va.To(p.PropertyType)
						).Compile<Action<T, object>>(Arg,va);
					return (o, vi) =>
						func(o, Convert.ChangeType(vi, p.PropertyType));
				})
				);

			public static Func<T, string[]> FuncSerialize;
			static Func<string[], T> FuncDeserialize;
			public static Func<T,string>[] GetGetters(string[] Props)
			{
				return Props.Select(p => Getters[p]).Select(g=>g.Value).ToArray();
			}
			public static Action<T,string>[] GetSetters(string[] Props)
			{
				return Props.Select(p => Setters[p]).Select(g => g.Value).ToArray();
			}
			
		}

		public static PropertyInfo[] TableColumns<T>()
			=> TableSerializationHelper<T>.Props;

		public static IEnumerable<IEnumerable<string>> TableSerialize<T>(
			string[] Props, 
			IEnumerable<T> Items
			)
		{
			var getters = TableSerializationHelper<T>.GetGetters(Props);
			foreach(var i in Items)
			{
				yield return getters.Select(g => g(i));
			}
		}

		public static IEnumerable<T> TableDeserialize<T>(
			string[] Props, 
			IEnumerable<IEnumerable<string>> Rows
			)
			where T:new()
		{
			var setters = TableSerializationHelper<T>.GetSetters(Props);
			foreach (var r in Rows)
			{
				var re = new T();
				var i = 0;
				foreach(var v in r)
				{
					if (i == setters.Length)
						break;
					setters[i](re, v);
				}
				yield return re;
			}
		}
	}
}

