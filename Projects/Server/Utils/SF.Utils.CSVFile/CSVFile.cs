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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Utils
{
	
	public class CSVFile 
	{
		public static IEnumerable<List<string>> ParseLines(string Content)
		{
			var re = new List<string>();
			foreach (var value in ParseValues(Content))
			{
				if (value == null)
				{
					yield return re;
					re.Clear();
				}
				else
					re.Add(value);
			}
			if (re.Count > 0)
				yield return re;

		}
		public static IEnumerable<string> ParseValues(string Content)
		{
			var i = 0;
			var e = Content.Length;
			var inString = false;
			var strBuilder = new StringBuilder();
			var valueReturned=false;
			while (i < e)
			{
				if (inString)
				{
					var t = Content.IndexOf('"', i);
					if (t != -1 && t <= e - 1 && Content[t + 1] == '"')
					{
						strBuilder.Append(Content, i, t - i + 1);
						i = t + 2;
						continue;
					}
					var te = t == -1 ? e : t;
					if (strBuilder.Length > 0)
					{
						strBuilder.Append(Content, i, te - i);
						yield return strBuilder.ToString();
						strBuilder.Clear();
					}
					else
						yield return Content.Substring(i, te - i);
					inString = false;
					i = te + 1;
					valueReturned = true;
					continue;
				}
				var c = Content[i];
				switch (c)
				{
					case ',':
						if (valueReturned)
							valueReturned = false;
						else
							yield return "";
						i++;
						continue;
					case '"':
						i++;
						inString = true;
						continue;
					default:
						if (c == '\n')
						{
							i++;
							if (valueReturned)
								valueReturned = false;
							else
								yield return "";
							yield return null;
							continue;
						}

						if (char.IsWhiteSpace(c))
						{
							i++;
							continue;
						}

						var t = Content.IndexOfAny(ValueStopChars, i);
						var te = t == -1 ? e : t;
						yield return Content.Substring(i, te - i);
						i = te ;
						valueReturned = true;
						break;
				}
			}
			yield return null;
		}
		static readonly char[] ValueStopChars = new[] { '\r', '\n', ',' };


		string Content { get; }
		int BodyBegin { get; }
		public string[] Columns { get; }

		IEnumerator<List<string>> _LineParser;
		public CSVFile(string Content,bool WithHeaders)
		{
			_LineParser = ParseLines(Content).GetEnumerator();
			if (WithHeaders)
			{
				if (_LineParser.MoveNext())
					Columns = _LineParser.Current.ToArray();
			}
		}

		public IEnumerable<IReadOnlyList<string>> Rows
		{
			get
			{
				while (_LineParser.MoveNext())
					yield return _LineParser.Current;
			}
		}
		static char[] EscChars { get; } = new[] { ' ', '\n', '\r' };
		public static async Task Write(System.IO.TextWriter writer,IEnumerable<string> Row)
		{
			var first = true;
			foreach (var v in Row)
			{
				if (first)
					first = false;
				else
					await writer.WriteAsync(',');
				if (v == null)
				{ }
				else if (v.IndexOfAny(EscChars) == -1)
					await writer.WriteAsync(v);
				else
				{
					await writer.WriteAsync('\"');
					var i = 0;
					for (; ; )
					{
						var t = v.IndexOf('"');
						var l = t == -1 ? v.Length - i : t - i;
						await writer.WriteAsync(v.Substring(i, l));
						if (t == -1) break;
						await writer.WriteAsync("\"\"");
						i = t + 1;
					}
					await writer.WriteAsync('\"');
				}
				
			}
			await writer.WriteLineAsync();
		}
		
					
	}
}

