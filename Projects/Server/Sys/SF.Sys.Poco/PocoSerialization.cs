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
			static PropertyInfo[] Props { get; } =
				typeof(T).AllPublicInstanceProperties()
				.Where(p =>
					p.PropertyType.IsConstType() ||
					(p.PropertyType.GetGenericArgumentTypeAsNullable()?.IsConstType() ?? false)
				)
				.ToArray();
			public static string[] Columns { get; } = Props.Select(p => p.Comment().Name).ToArray();
			public static Func<T, string[]> FuncSerialize;
			static Func<string[], T> FuncDeserialize;

			public static string[] Serialize(T Item)
			{
				return null;
			}
			public static T Deserialize(string[] Columns, string[] Values)
			{
				return default;
			}
		}

		public static string[] TableColumns<T>()
			=> TableSerializationHelper<T>.Columns;

		public static IEnumerable<string[]> TableSerialize<T>(IEnumerable<T> Items)
			=> Items.Select(TableSerializationHelper<T>.Serialize);

		public static IEnumerable<T> TableDeserialize<T>(
			string[] Columns, 
			IEnumerable<IEnumerable<string>> Rows
			)
		{
			foreach(var r in Rows)
			{
				var vs = r.ToArray();
				yield return TableSerializationHelper<T>.Deserialize(Columns, vs);
			}
		}
	}
}

