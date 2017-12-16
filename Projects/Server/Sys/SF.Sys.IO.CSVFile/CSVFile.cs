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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SF.Sys.IO
{
	public class CSFile : IEnumerable<string[]>
	{
		public string[] Headers { get; }

		static char[] splitters = new[] { '"', ',' };

		(int,int,bool) ParseLine(string content,int begin,int end,int line)
		{
			var re = new List<(int,int)>();
			var i = begin;
			for (; ; )
			{
				//当前值带有引号
				if (i < end && content[0]=='"')
				{
					
				}
				var ci = content.IndexOf(',', i, end - i);
				var ve = ci == -1 ? end : ci;
				re.Add((i, ve));
				if (ci == -1)
					break;

				var c = content[ci];
				if (c == ',')
				{
					re.Add((i, ci));
					i = ci + 1;
					continue;
				}

				//找到最后的双引号
				var hasInnerQuot=false;
				ci++;
				for(; ; )
				{
					var t = content.IndexOf('"', ci, end - ci);
					if (t == -1)
						throw new PublicArgumentException($"CSV格式错误,找不到单元格末尾：行:{line},字符:{ci - begin}");
					
					//找到单元格末尾
					if (t==end-1 || t<end-1 && content[t+1]==',')
					{
						re.Add((i,))
					}


				}



				var e = ci == -1 ? end : ci;
				var qi = content.IndexOf('"',e);
				if (qi == -1)
					re.Add(())
				{

				}
			}
		}

		public CSFile(string Content,bool WithHeaders)
		{

		}

		public IEnumerator<string[]> GetEnumerator()
		{
		
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

