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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SF.Sys.IO
{
	
	public class CSVFile 
	{
		string Content { get;  }
		int BodyBegin { get;  }
		public string[] Columns { get; }
		
		public CSVFile(string Content,bool WithHeaders)
		{
			this.Content = Content;
			if (WithHeaders)
			{
				var i = Content.IndexOf("\r\n", System.StringComparison.Ordinal);
				if (i == -1)
					throw new ArgumentException("找不到文件头部");
				Columns = ParseLine(Content, 0, i, 1).ToArray();
				BodyBegin = i + 2;
			}
		}
		
		public IEnumerable<IEnumerable<string>> Rows
		{
			get
			{
				var line = Columns == null ? 1 : 2;
				var i = BodyBegin;
				for (; ; )
				{
					var t = Content.IndexOf("\r\n", i, System.StringComparison.Ordinal);
					var e = t == -1 ? Content.Length : t;
					if (e > i)
					{
						yield return ParseLine(Content, i, e, line);
					}
					if (t == -1)
						break;
					line++;
					i = t + 2;
				}
			}
		}
		static string ParseValue(string Content, int begin, int end, bool withQuotes)
		{
			var re = Content.Substring(begin, end - begin);
			if (withQuotes)
				return re.Replace("\"\"", "\"");
			else
				return re;
		}
		static IEnumerable<string> ParseLine(string content, int begin, int end, int line)
		{
			var i = begin;
			for (; ; )
			{
				//当前值带有引号
				if (i == end)
				{
					yield return ParseValue(content,i, end, false);
					break;
				}

				if (content[0] != '"')
				{
					var ci = content.IndexOf(',', i, end - i);
					var ve = ci == -1 ? end : ci;
					yield return ParseValue(content, i, ve, false);
					if (ci == -1)
						break;
					i = ci + 1;
					continue;
				}

				i++;
				var t = i;
				var hasInnerQuote = false;
				for (; ; )
				{
					var ci = content.IndexOf('"', t, end - t);
					if (ci == -1)
						throw new ArgumentException($"CSV格式错误,找不到单元格末尾：行:{line},字符:{ci - begin}");
					if (ci == end - 1)
					{
						yield return ParseValue(content, i, ci, hasInnerQuote);
						i = end;
						break;
					}
					var c = content[ci + 1];
					if (c == ',')
					{
						yield return ParseValue(content, i, ci, hasInnerQuote);
						i = ci + 2;
						break;
					}
					else if (c == '"')
					{
						hasInnerQuote = true;
						t = ci + 2;
					}
					else
						throw new ArgumentException($"CSV格式错误,单元格末尾发现错误的字符：行:{line},字符:{ci - begin + 1}");
				}
				if (i == end)
					break;
			}
		}

	}
}

