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

namespace SF.Sys
{
	public static class Strings
	{
		public static string Numbers { get; } = "0123456789";
		public static string LowerChars { get; } = "abcdefghijklmnopqrstuvwxyz";
		public static string UpperChars { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public static string NumberAndLowerChars { get; } = Numbers + LowerChars;
		public static string NumberAndUpperChars { get; } = Numbers + UpperChars;
		public static string NumberAndLowerUpperChars { get; } = Numbers + LowerChars + UpperChars;
		public static string LowerHexChars { get; } = Numbers + "abcdef";
		public static string UpperHexChars { get; } = Numbers + "ABCDEF";
	}

	public static class StringExtension
	{
		public static byte[] Base64(this string str, bool UriSafe = false)
		{
			if(UriSafe)
			{
				str = str.Replace('-', '+').Replace('_', '/');
				var l = str.Length % 4;
				if (l > 0) str += new string('=', 4 - l);
			}
			return Convert.FromBase64String(str);
		}
		public static string Random(this string seed,int length,Random rand=null)
		{
			if (rand == null)
                rand = RandomFactory.Create();
			var sl = seed.Length;
			var cs = new char[length];
            lock (rand)
            {
                for (var i = 0; i < length; i++)
                    cs[i] = seed[rand.Next(sl)];
            }
			return new string(cs);
		}
		
		public static byte[] UTF8Bytes(this string str)
		{
			if (str == null) return null;
			return Encoding.UTF8.GetBytes(str);
		}
		public static bool HasContent(this string str)
		{
			return !string.IsNullOrWhiteSpace(str);
		}
		public static string EnsureStart(this string str,string start)
		{
			return str.StartsWith(start) ? str : start + str;
		}
		public static string EnsureEnd(this string str, string end)
		{
			return str.EndsWith(end) ? str : str+end;
		}
		public static string TrimEnd(this string str,string end)
		{
			if (str.EndsWith(end))
				return str.Substring(0, str.Length - end.Length);
			return str;
		}
		public static string TrimStart(this string str, string start)
		{
			if (str.StartsWith(start))
				return str.Substring(start.Length);
			return str;
		}
		public static string TrimEndTo(this string str, string end, bool trimEnd=true,bool trimToLastEnd=false)
		{
			var i = trimToLastEnd ? str.IndexOf(end) : str.LastIndexOf(end);
			if (i == -1) return str;
			return str.Substring(0, trimEnd ? i : i + end.Length);
		}
		public static string TrimStartTo(this string str, string start, bool trimStart = true,bool trimToLastStart=false)
		{
			var i = trimToLastStart ? str.LastIndexOf(start) : str.IndexOf(start);
			if (i == -1) return str;
			return str.Substring(trimStart ? i + start.Length : i);
		}
		public unsafe static int GetConsistencyHashCode(this string str)
		{
			if (str == null)
				return 0;
			fixed (char* src = str)
			{
				int hash1 = (5381 << 16) + 5381;
				int hash2 = hash1;

				int* pint = (int*)src;
				int len = str.Length;
				while (len > 0)
				{
					hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
					if (len <= 2)
					{
						break;
					}
					hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
					pint += 2;
					len -= 4;
				}
				return hash1 + (hash2 * 1566083941);
			}
		}
		public static string LetterOrDigits(this string s)
		{
			if (string.IsNullOrEmpty(s)) return s;
			var sb = new StringBuilder(s.Length);
			foreach(var c in s)
			{
				if (char.IsLetterOrDigit(c))
					sb.Append(c);
			}
			return sb.ToString();
		}
		public static string Limit(this string s,int length)
		{
			if (s == null)
				return s;
			if (s.Length <= length)
				return s;
			return s.Substring(0, length);
		}
		public static (string, string) Split2(this string s,char c)
		{
			if (s == null)
				return ((string)null, (string)null);
			var i = s.IndexOf(c);
			if (i == -1)
				return (s, (string)null);
			return (s.Substring(0, i), s.Substring(i + 1));
		}
        public static IEnumerable<string> SplitAndNormalizae(this string s,char c=';')
        {
            if (string.IsNullOrEmpty(s))
                return Enumerable.Empty<string>();
            return s.Split(c).Select(i => i.Trim()).Where(i => i.Length > 0).Distinct();
        }
		public static (string, string) LastSplit2(this string s, char c)
		{
			if (s == null)
				return ((string)null, (string)null);
			var i = s.LastIndexOf(c);
			if (i == -1)
				return ((string)null,s);
			return (s.Substring(0, i), s.Substring(i + 1));
		}
		public static bool IsNullOrEmpty(this string s)
			=> string.IsNullOrEmpty(s);
		public static bool IsNullOrWhiteSpace(this string s)
			=> string.IsNullOrWhiteSpace(s);

		public static string Substring(this string s,string Head,int HeadOffset,string Tail,int TailOffset)
		{
			var i = s.IndexOf(Head);
			if (i == -1)
				return string.Empty;
			i += Head.Length + HeadOffset;
			var j = s.LastIndexOf(Tail);
			if (j == -1)
				return string.Empty;
			j += TailOffset;
			return s.Substring(i, j - i);
		}
		public static sbyte ToInt8(this string str) => sbyte.Parse(str);
		public static short ToInt16(this string str) => short.Parse(str);
		public static int ToInt32(this string str) => int.Parse(str);
		public static long ToInt64(this string str) => long.Parse(str);
		public static byte ToUInt8(this string str) => byte.Parse(str);
		public static ushort ToUInt16(this string str) => ushort.Parse(str);
		public static uint ToUInt32(this string str) => uint.Parse(str);
		public static ulong ToUInt64(this string str) => ulong.Parse(str);
		public static bool ToBoolean(this string str) => bool.Parse(str);
		public static double ToDouble(this string str) => double.Parse(str);
		public static float ToFloat(this string str) => float.Parse(str);
		public static decimal ToDecimal(this string str) => decimal.Parse(str);
		public static DateTime ToDateTime(this string str, string Format, System.Globalization.DateTimeStyles Styles = System.Globalization.DateTimeStyles.AssumeLocal)
			=> DateTime.ParseExact(str, Format, null, Styles);

		public static sbyte? TryToInt8(this string str) => sbyte.TryParse(str,out var re)?(sbyte?)re:null;
		public static short? TryToInt16(this string str) => short.TryParse(str, out var re) ? (short?)re : null;
		public static int? TryToInt32(this string str) => int.TryParse(str, out var re) ? (int?)re : null;
		public static long? TryToInt64(this string str) => long.TryParse(str, out var re) ? (long?)re : null;
		public static byte? TryToUInt8(this string str) => byte.TryParse(str, out var re) ? (byte?)re : null;
		public static ushort? TryToUInt16(this string str) => ushort.TryParse(str, out var re) ? (ushort?)re : null;
		public static uint? TryToUInt32(this string str) => uint.TryParse(str, out var re) ? (uint?)re : null;
		public static ulong? TryToUInt64(this string str) => ulong.TryParse(str, out var re) ? (ulong?)re : null;
		public static bool? TryToBoolean(this string str) => bool.TryParse(str, out var re) ? (bool?)re : null;
		public static double? TryToDouble(this string str) => double.TryParse(str, out var re) ? (double?)re : null;
		public static float? TryToFloat(this string str) => float.TryParse(str, out var re) ? (float?)re : null;
		public static decimal? TryToDecimal(this string str) => decimal.TryParse(str, out var re) ? (decimal?)re : null;
		public static DateTime? TryToDateTime(this string str, string Format, System.Globalization.DateTimeStyles Styles = System.Globalization.DateTimeStyles.AssumeLocal)
					=> DateTime.TryParseExact(str, Format, null, Styles,out var re)?(DateTime?)re:null;


		public static void Escape(this string s,int offset,int length,bool[] escChars,Func<char,string> charEscaper,StringBuilder output)
		{
			var lastIndex = offset;
			var endIndex = offset + length;
			for (int i = offset ; i < endIndex; i++)
			{
				var c = s[i];
				if (c > 0 && c < 256 && escChars[c])
				{
					if (i > lastIndex)
						output.Append(s, lastIndex, i - lastIndex);
					output.Append(charEscaper(c));
					lastIndex = i + 1;
				}
			}
			if (lastIndex < endIndex)
				output.Append(s, lastIndex, endIndex - lastIndex);
		}
		public static void Unescape(this string s,int offset,int length,char escChar,Func<string,int,(int,char)> charUnescaper,StringBuilder output)
		{
			var i = offset;
			var end = offset + length;
			for(; ; )
			{
				var t = s.IndexOf(escChar, i, end - i);
				var l = t == -1 ? end - i : t - i;
				if (l > 0)
					output.Append(s, i, l);
				if (t == -1)
					break;
				var (r, c) = charUnescaper(s, t + 1);
				output.Append(c);
				i = t + 1 + r;
			}
		}


		static readonly System.Text.RegularExpressions.Regex _reg_replace =
			new System.Text.RegularExpressions.Regex("\\{([^:\\}]+)(:[^:\\}]+)?(:[^:\\}]+)?\\}");

		public static string Replace(this string tmpl, IReadOnlyDictionary<string, object> args)
		{
			if (tmpl == null) return null;
			return _reg_replace.Replace(tmpl, match =>
			{
				var grps = match.Groups;
				var gc = grps.Count;
				var key = grps[1].Value;
				
				if (args.TryGetValue(key, out object re) && re != null)
				{
					var format = gc > 2 && grps[2].Value.HasContent() ? grps[2].Value.Substring(1) : null;
					if (format != null && re is IFormattable f)
						return f.ToString(format, null);
					else
						return re.ToString();
				}
				return gc > 3 && grps[3].Value.HasContent() ? grps[3].Value.Substring(1) : string.Empty;
			});
		}
	}
}
