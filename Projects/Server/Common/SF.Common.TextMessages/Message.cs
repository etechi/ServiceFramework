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

namespace SF.Common.TextMessages
{
	
	public class Message
	{
		public string Title { get; set; }
		public string Body { get; set; }
		public string Sender { get; set; }
        public string TrackEntityId { get; set; }
        public IDictionary<string, string> Arguments { get; set; }
        public IDictionary<string, string> Headers { get; set; }

		public override string ToString()
		{
            var sb = new StringBuilder();
            sb.Append("发信人:");
            sb.Append(Sender);

            sb.Append(" 跟踪:");
            sb.Append(TrackEntityId);

            sb.Append(" 标题:");
            sb.Append(Title??"");
            if (Headers != null && Headers.Count != 0)
            {
                sb.Append(" 头部:");
                sb.Append(string.Join(";",Headers.Select(h=>h.Key+"="+h.Value)));
            }
            sb.Append(" 正文:");
            sb.Append(Body??"");

            if (Arguments!= null && Arguments.Count != 0)
            {
                sb.Append(" 参数:");
				sb.Append(string.Join(";", Arguments.Select(h => h.Key + "=" + h.Value)));
			}
			return sb.ToString();
        }
	}
}
