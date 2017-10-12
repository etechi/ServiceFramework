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

namespace SF.Entities
{
	public enum SortOrder
	{
		Default,
		Asc,
		Desc,
		Random
	}
    public class Paging
	{
		public int Offset { get; set; }
		public int Count { get; set; }
		public string SortMethod { get; set; }
		public SortOrder SortOrder { get; set; }
		public bool TotalRequired { get; set; }
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
			IEnumerable<KeyValuePair<string,string>> attrs,
			int defaultLimit,
            bool summarySupport=false,
			string defaultMethod=null,
			SortOrder defaultOrder=SortOrder.Default,
			bool totalRequired=false
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
					case "_po":offset = p.Value;break;
					case "_pl":count = p.Value;break;
					case "_pm":sortMethod = p.Value;break;
					case "_ps":sortOrder = p.Value;break;
					case "_pt":totalRequired =p.Value=="1";break;
                    case "_pa":summaryRequired = summarySupport && p.Value == "1";break;

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
				TotalRequired= totalRequired,
                SummaryRequired= summaryRequired,
                SortMethod = string.IsNullOrWhiteSpace(sortMethod) ? defaultMethod : sortMethod,
				SortOrder = 
					sortOrder == "desc" ? SortOrder.Desc : 
					sortOrder == "asc" ? SortOrder.Asc : 
					sortOrder == "rand"?SortOrder.Random:
					defaultOrder
			};
		}
	}
}
