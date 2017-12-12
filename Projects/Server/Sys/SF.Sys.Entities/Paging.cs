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

using SF.Sys.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Entities
{
	/// <summary>
	/// 排序类型
	/// </summary>
	public enum SortOrder
	{
		/// <summary>
		/// 默认
		/// </summary>
		Default,
		/// <summary>
		/// 升序
		/// </summary>
		Asc,
		/// <summary>
		/// 降序
		/// </summary>
		Desc,
		/// <summary>
		/// 随机排序
		/// </summary>
		Random
	}
	public class Paging
	{
		/// <title>起始记录</title>
		/// <summary>
		/// 起始记录,从0开始
		/// </summary>
		public int Offset { get; set; }
		/// <summary>
		/// 返回记录条数
		/// </summary>
		public int Count { get; set; }
		/// <summary>
		/// 排序方式
		/// </summary>
		public string SortMethod { get; set; }

		/// <summary>
		/// 排序类型
		/// </summary>
		public SortOrder SortOrder { get; set; }

		/// <summary>
		/// 返回总数
		/// </summary>
		public bool TotalRequired { get; set; }
		/// <summary>
		/// 返回摘要
		/// </summary>
		public bool SummaryRequired { get; set; }

		public static Paging Single => new Paging
		{
			Count = 1
		};
		public static Paging Default => new Paging
		{
			Count = 100
		};
		public static Paging All => new Paging
		{
			Count = 10000
		};
		public static Paging Create(
			IEnumerable<KeyValuePair<string, string>> attrs,
			int defaultLimit,
			bool summarySupport = false,
			string defaultMethod = null,
			SortOrder defaultOrder = SortOrder.Default,
			bool totalRequired = false
			)
		{
			string offset = null;
			string count = null;
			string sortMethod = null;
			string sortOrder = null;
			var summaryRequired = false;
			foreach (var p in attrs)
			{
				switch (p.Key)
				{
					case "_po": offset = p.Value; break;
					case "_pl": count = p.Value; break;
					case "_pm": sortMethod = p.Value; break;
					case "_ps": sortOrder = p.Value; break;
					case "_pt": totalRequired = p.Value == "1"; break;
					case "_pa": summaryRequired = summarySupport && p.Value == "1"; break;

				}
			}
			int o;
			if (offset == null)
				o = 0;
			else if (!int.TryParse(offset, out o))
				throw new ArgumentException();

			int l;
			if (count == null)
				l = defaultLimit;
			else if (!int.TryParse(count, out l))
				throw new ArgumentException();

			return new Paging
			{
				Count = l,
				Offset = o,
				TotalRequired = totalRequired,
				SummaryRequired = summaryRequired,
				SortMethod = string.IsNullOrWhiteSpace(sortMethod) ? defaultMethod : sortMethod,
				SortOrder =
					sortOrder == "desc" ? SortOrder.Desc :
					sortOrder == "asc" ? SortOrder.Asc :
					sortOrder == "rand" ? SortOrder.Random :
					defaultOrder
			};
		}
	}
	public interface IPagingArgument
	{
		[Ignore]
		Paging Paging { get; set; }
	}
	/// <summary>
	/// 分页配置
	/// </summary>
	public class PagingArgument  : IPagingArgument
	{
		/// <summary>
		/// 分页参数
		/// </summary>
		public Paging Paging { get; set; }
	}
	public static class PagingArgumentExtension
	{
		public static TArgument WithPaging<TArgument>(this TArgument arg,Paging Paging) where TArgument:IPagingArgument
		{
			arg.Paging = Paging;
			return arg;
		}
		public static TArgument WithDefaultPaging<TArgument>(this TArgument arg, Paging Paging) where TArgument : IPagingArgument
		{
			if(arg.Paging==null)
				arg.Paging = Paging;
			return arg;
		}
	}
}
