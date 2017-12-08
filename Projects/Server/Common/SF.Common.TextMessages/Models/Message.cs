﻿#region Apache License Version 2.0
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

namespace SF.Common.TextMessages
{
	
	public class Message
	{
		/// <summary>
		/// 消息策略
		/// </summary>
		public string Policy { get; set; }

		/// <summary>
		/// 跟踪对象
		/// </summary>
        public string TrackEntityId { get; set; }

		/// <summary>
		/// 参数
		/// </summary>
        public IReadOnlyDictionary<string, object> Arguments { get; set; }

		public override string ToString()
		{
            var sb = new StringBuilder();
			sb.Append($" {Policy} 跟踪:{TrackEntityId}");
            
            if (Arguments!= null && Arguments.Count != 0)
            {
                sb.Append(" 参数:");
				sb.Append(string.Join(";", Arguments.Select(h => h.Key + "=" + h.Value)));
			}
			return sb.ToString();
        }
	}
}
