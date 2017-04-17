using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.System.TextMessages
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
