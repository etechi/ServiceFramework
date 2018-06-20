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
		/// <summary>
		/// 数字
		/// </summary>
		public static string Numbers { get; } = "0123456789";
		/// <summary>
		/// 小写字母
		/// </summary>
		public static string LowerChars { get; } = "abcdefghijklmnopqrstuvwxyz";
		/// <summary>
		/// 大写字母
		/// </summary>
		public static string UpperChars { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		/// <summary>
		/// 数字和小写字母
		/// </summary>
		public static string NumberAndLowerChars { get; } = Numbers + LowerChars;
		/// <summary>
		/// 数字和大写字母
		/// </summary>
		public static string NumberAndUpperChars { get; } = Numbers + UpperChars;
		/// <summary>
		/// 数字和大小写字母
		/// </summary>
		public static string NumberAndLowerUpperChars { get; } = Numbers + LowerChars + UpperChars;
		/// <summary>
		/// 小写十六进制字母
		/// </summary>
		public static string LowerHexChars { get; } = Numbers + "abcdef";
		/// <summary>
		/// 大写十六进制字母
		/// </summary>
		public static string UpperHexChars { get; } = Numbers + "ABCDEF";
	}

	public static class StringExtension
	{
		/// <summary>
		/// Base64编码
		/// </summary>
		/// <param name="str">字符串</param>
		/// <param name="UriSafe">是否编码为Uri安全字符串</param>
		/// <returns>base64字节</returns>
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
		/// <summary>
		/// 生成随机字符串
		/// </summary>
		/// <param name="seed">构成随机字符串的字符</param>
		/// <param name="length">随机字符串长度</param>
		/// <param name="rand">随机数生成器</param>
		/// <returns>随机字符串</returns>
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
		
		/// <summary>
		/// Utf8编码
		/// </summary>
		/// <param name="str">被编码字符串</param>
		/// <returns>utf8字节</returns>
		public static byte[] UTF8Bytes(this string str)
		{
			if (str == null) return null;
			return Encoding.UTF8.GetBytes(str);
		}
		/// <summary>
		/// 检查字符串是否有内容
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>是否有内容</returns>
		public static bool HasContent(this string str)
		{
			return !string.IsNullOrWhiteSpace(str);
		}
		/// <summary>
		/// 检查字符串是否为指定的开始，若不是，则在开始初添加
		/// </summary>
		/// <param name="str">字符串</param>
		/// <param name="start">指定开始</param>
		/// <returns>包含指定开始的字符串</returns>
		public static string EnsureStart(this string str,string start)
		{
			return str.StartsWith(start) ? str : start + str;
		}
		/// <summary>
		/// 检查字符串是否为指定的结尾，若不是，则在结尾初添加
		/// </summary>
		/// <param name="str">字符串</param>
		/// <param name="end">指定结尾</param>
		/// <returns>包含指定结尾的字符串</returns>
		public static string EnsureEnd(this string str, string end)
		{
			return str.EndsWith(end) ? str : str+end;
		}
		/// <summary>
		/// 若字符串包含指定结尾，则去除之
		/// </summary>
		/// <param name="str">字符串</param>
		/// <param name="end">结尾</param>
		/// <returns>去除了指定结尾的字符串</returns>
		public static string TrimEnd(this string str,string end)
		{
			if (str.EndsWith(end))
				return str.Substring(0, str.Length - end.Length);
			return str;
		}
		/// <summary>
		/// 若字符串包含指定开始，则去除之
		/// </summary>
		/// <param name="str">字符串</param>
		/// <param name="start">开始</param>
		/// <returns>去除了指定开始的字符串</returns>
		public static string TrimStart(this string str, string start)
		{
			if (str.StartsWith(start))
				return str.Substring(start.Length);
			return str;
		}
		
		/// <summary>
		/// 获取字符串Hash，原生的String.GetHashCode在不同环境下返回的结果可能不同
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>hash值</returns>
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
		/// <summary>
		/// 返回默认字符串
		/// </summary>
		/// <param name="s">字符串</param>
		/// <param name="Default">默认值</param>
		/// <returns>默认字符串</returns>
		public static string WithDefault(this string s,string Default)
		{
			if (s.HasContent())
				return s;
			return Default;
		}
		/// <summary>
		/// 限制字符串长度
		/// </summary>
		/// <param name="s">字符串</param>
		/// <param name="length">最大长度</param>
		/// <returns>已限制长度的字符串</returns>
		public static string Limit(this string s,int length)
		{
			if (s == null)
				return s;
			if (s.Length <= length)
				return s;
			return s.Substring(0, length);
		}
		/// <summary>
		/// 返回指定字符左侧的字符串
		/// </summary>
		/// <param name="s">原字符串</param>
		/// <param name="c">字符</param>
		/// <param name="withSplitter">是否包含字符</param>
		/// <returns>指定字符左侧的字符串</returns>
		public static string LeftBefore(this string s,char c,bool withSplitter = false)
		{
			var i = s?.IndexOf(c) ?? -1;
			if (i == -1) return s;
			return s.Substring(0, withSplitter ? i+1:i);
		}
		/// <summary>
		/// 返回指定字符有侧的字符串
		/// </summary>
		/// <param name="s">原字符串</param>
		/// <param name="c">字符</param>
		/// <param name="withSplitter">是否包含字符</param>
		/// <returns>指定字符右侧的字符串</returns>
		public static string RightAfter(this string s, char c, bool withSplitter = false)
		{
			var i = s?.IndexOf(c) ?? -1;
			if (i == -1) return s;
			return s.Substring(withSplitter ? i : i+1);
		}
		/// <summary>
		/// 返回指定字符窜左侧的字符串
		/// </summary>
		/// <param name="s">原字符串</param>
		/// <param name="sp">字符窜</param>
		/// <param name="withSplitter">是否包含字符</param>
		/// <returns>指定字符左侧的字符串</returns>
		public static string LeftBefore(this string s, string sp, bool withSplitter = false)
		{
			if (sp == null) throw new ArgumentNullException();
			var i = s?.IndexOf(sp) ?? -1;
			if (i == -1) return s;
			return s.Substring(0, withSplitter ? i + sp.Length : i );
		}
		/// <summary>
		/// 返回指定字符窜右侧的字符串
		/// </summary>
		/// <param name="s">原字符串</param>
		/// <param name="sp">字符窜</param>
		/// <param name="withSplitter">是否包含字符窜</param>
		/// <returns>指定字符窜右侧的字符串</returns>
		public static string RightAfter(this string s, string sp, bool withSplitter = false)
		{
			if (sp == null) throw new ArgumentNullException();
			var i = s?.IndexOf(sp) ?? -1;
			if (i == -1) return s;
			return s.Substring(withSplitter ? i  : i + sp.Length);
		}


		/// <summary>
		/// 返回指定最后字符左侧的字符串
		/// </summary>
		/// <param name="s">原字符串</param>
		/// <param name="c">字符</param>
		/// <param name="withSplitter">是否包含字符</param>
		/// <returns>指定最后字符左侧的字符串</returns>
		public static string LeftBeforeLast(this string s, char c, bool withSplitter = false)
		{
			var i = s?.LastIndexOf(c) ?? -1;
			if (i == -1) return s;
			return s.Substring(0, withSplitter ? i + 1 : i );
		}

		/// <summary>
		/// 返回指定最后字符右侧的字符串
		/// </summary>
		/// <param name="s">原字符串</param>
		/// <param name="c">字符</param>
		/// <param name="withSplitter">是否包含字符</param>
		/// <returns>指定最后字符右侧的字符串</returns>
		public static string RightAfterLast(this string s, char c, bool withSplitter = false)
		{
			var i = s?.LastIndexOf(c) ?? -1;
			if (i == -1) return s;
			return s.Substring(withSplitter ? i  : i + 1);
		}

		/// <summary>
		/// 返回指定最后字符窜左侧的字符串
		/// </summary>
		/// <param name="s">原字符串</param>
		/// <param name="sp">字符窜</param>
		/// <param name="withSplitter">是否包含字符窜</param>
		/// <returns>指定最后字符窜左侧的字符串</returns>
		public static string LeftBeforeLast(this string s, string sp, bool withSplitter = true)
		{
			if (sp == null) throw new ArgumentNullException();
			var i = s?.LastIndexOf(sp) ?? -1;
			if (i == -1) return s;
			return s.Substring(0, withSplitter ? i + sp.Length : i );
		}

		/// <summary>
		/// 返回指定最后字符窜右侧的字符串
		/// </summary>
		/// <param name="s">原字符串</param>
		/// <param name="sp">字符窜</param>
		/// <param name="withSplitter">是否包含字符窜</param>
		/// <returns>指定最后字符窜右侧的字符串</returns>
		public static string RightAfterLast(this string s, string sp, bool withSplitter = true)
		{
			if (sp == null) throw new ArgumentNullException();
			var i = s?.LastIndexOf(sp) ?? -1;
			if (i == -1) return s;
			return s.Substring(withSplitter ? i  : i + sp.Length);
		}

		/// <summary>
		/// 按照指定字符将字符串分割成两部分
		/// </summary>
		/// <param name="s">字符串</param>
		/// <param name="c">字符</param>
		/// <returns>分割后的字符串</returns>
		public static (string, string) Split2(this string s,char c)
		{
			if (s == null)
				return ((string)null, (string)null);
			var i = s.IndexOf(c);
			if (i == -1)
				return (s, (string)null);
			return (s.Substring(0, i), s.Substring(i + 1));
		}
		/// <summary>
		/// 使用字符串分割，并规则化
		/// </summary>
		/// <param name="s">字符串</param>
		/// <param name="c">分隔符</param>
		/// <returns>字符串枚举</returns>
        public static IEnumerable<string> SplitAndNormalizae(this string s,char c=';')
        {
            if (string.IsNullOrEmpty(s))
                return Enumerable.Empty<string>();
            return s.Split(c).Select(i => i.Trim()).Where(i => i.Length > 0).Distinct();
        }
		/// <summary>
		/// 按照指定最后字符将字符串分割成两部分
		/// </summary>
		/// <param name="s">字符串</param>
		/// <param name="c">字符</param>
		/// <param name="start">其实位置</param>
		/// <returns>分割后的字符串</returns>
		public static (string, string) LastSplit2(this string s, char c,int? start=null)
		{
			if (s == null)
				return ((string)null, (string)null);
			var i = start.HasValue ? s.LastIndexOf(c, start.Value) : s.LastIndexOf(c);
			if (i == -1)
				return ((string)null,s);
			return (s.Substring(0, i), s.Substring(i + 1));
		}
		/// <summary>
		/// 判断字符串是否为空或空串
		/// </summary>
		/// <param name="s">字符串</param>
		/// <returns>字符串是否为空或空串</returns>
		public static bool IsNullOrEmpty(this string s)
			=> string.IsNullOrEmpty(s);

		/// <summary>
		/// 判断字符串是否为空或空串、空白
		/// </summary>
		/// <param name="s">字符串</param>
		/// <returns>字符串是否为空或空串、空白</returns>
		public static bool IsNullOrWhiteSpace(this string s)
			=> string.IsNullOrWhiteSpace(s);

		/// <summary>
		/// 截去字符串头尾
		/// </summary>
		/// <param name="s">字符串</param>
		/// <param name="Head">头部标记</param>
		/// <param name="HeadOffset">头部偏移</param>
		/// <param name="Tail">尾部标记</param>
		/// <param name="TailOffset">尾部偏移</param>
		/// <returns>截取的字符串</returns>
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
		/// <summary>
		/// 转为Int8
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static sbyte ToInt8(this string str) => sbyte.Parse(str);
		/// <summary>
		/// 转为Int16
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static short ToInt16(this string str) => short.Parse(str);
		/// <summary>
		/// 转为Int32
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static int ToInt32(this string str) => int.Parse(str);
		/// <summary>
		/// 转为Int64
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static long ToInt64(this string str) => long.Parse(str);
		/// <summary>
		/// 转为UInt8
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static byte ToUInt8(this string str) => byte.Parse(str);
		/// <summary>
		/// 转为UInt16
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static ushort ToUInt16(this string str) => ushort.Parse(str);
		/// <summary>
		/// 转为UInt32
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static uint ToUInt32(this string str) => uint.Parse(str);
		/// <summary>
		/// 转为UInt64
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static ulong ToUInt64(this string str) => ulong.Parse(str);
		/// <summary>
		/// 转为Boolean
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static bool ToBoolean(this string str) => bool.Parse(str);
		/// <summary>
		/// 转为Double
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static double ToDouble(this string str) => double.Parse(str);
		/// <summary>
		/// 转为Float
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static float ToFloat(this string str) => float.Parse(str);
		/// <summary>
		/// 转为Decimal
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static decimal ToDecimal(this string str) => decimal.Parse(str);
		/// <summary>
		/// 转换为DateTime
		/// </summary>
		/// <param name="str">字符串</param>
		/// <param name="Format">格式</param>
		/// <param name="Styles">样式</param>
		/// <returns>是否成功</returns>
		public static DateTime ToDateTime(this string str, string Format, System.Globalization.DateTimeStyles Styles = System.Globalization.DateTimeStyles.AssumeLocal)
			=> DateTime.ParseExact(str, Format, null, Styles);

		/// <summary>
		/// 尝试转换为Int8
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>转换结果</returns>
		public static sbyte? TryToInt8(this string str) => sbyte.TryParse(str,out var re)?(sbyte?)re:null;
		/// <summary>
		/// 尝试转换为Int16
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>转换结果</returns>
		public static short? TryToInt16(this string str) => short.TryParse(str, out var re) ? (short?)re : null;
		/// <summary>
		/// 尝试转换为Int32
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>转换结果</returns>
		public static int? TryToInt32(this string str) => int.TryParse(str, out var re) ? (int?)re : null;
		/// <summary>
		/// 尝试转换为Int64
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>转换结果</returns>
		public static long? TryToInt64(this string str) => long.TryParse(str, out var re) ? (long?)re : null;
		/// <summary>
		/// 尝试转换为UInt8
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>转换结果</returns>
		public static byte? TryToUInt8(this string str) => byte.TryParse(str, out var re) ? (byte?)re : null;
		/// <summary>
		/// 尝试转换为UInt16
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>转换结果</returns>
		public static ushort? TryToUInt16(this string str) => ushort.TryParse(str, out var re) ? (ushort?)re : null;
		/// <summary>
		/// 尝试转换为UInt32
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>转换结果</returns>
		public static uint? TryToUInt32(this string str) => uint.TryParse(str, out var re) ? (uint?)re : null;
		/// <summary>
		/// 尝试转换为Decimal
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>转换结果</returns>
		public static ulong? TryToUInt64(this string str) => ulong.TryParse(str, out var re) ? (ulong?)re : null;
		/// <summary>
		/// 尝试转换为Boolean
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>转换结果</returns>
		public static bool? TryToBoolean(this string str) => bool.TryParse(str, out var re) ? (bool?)re : null;
		/// <summary>
		/// 尝试转换为Double
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>转换结果</returns>
		public static double? TryToDouble(this string str) => double.TryParse(str, out var re) ? (double?)re : null;
		/// <summary>
		/// 尝试转换为Float
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>转换结果</returns>
		public static float? TryToFloat(this string str) => float.TryParse(str, out var re) ? (float?)re : null;
		/// <summary>
		/// 尝试转换为Decimal
		/// </summary>
		/// <param name="str">字符串</param>
		/// <returns>转换结果</returns>
		public static decimal? TryToDecimal(this string str) => decimal.TryParse(str, out var re) ? (decimal?)re : null;
		/// <summary>
		/// 尝试转换为DateTime
		/// </summary>
		/// <param name="str">字符串</param>
		/// <param name="Format">格式</param>
		/// <param name="Styles">样式</param>
		/// <returns>是否成功</returns>
		public static DateTime? TryToDateTime(this string str, string Format, System.Globalization.DateTimeStyles Styles = System.Globalization.DateTimeStyles.AssumeLocal)
					=> DateTime.TryParseExact(str, Format, null, Styles,out var re)?(DateTime?)re:null;

		/// <summary>
		/// 转义字符串
		/// </summary>
		/// <param name="s">字符串</param>
		/// <param name="offset">起始位置</param>
		/// <param name="length">长度</param>
		/// <param name="escChars">须转义字符串</param>
		/// <param name="charEscaper">转义函数</param>
		/// <param name="output">输出内容</param>
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
		/// <summary>
		/// 反向转义字符串
		/// </summary>
		/// <param name="s">字符串</param>
		/// <param name="offset">起始位置</param>
		/// <param name="length">长度</param>
		/// <param name="escChar">转义字符串</param>
		/// <param name="charUnescaper">反向转义函数</param>
		/// <param name="output">输出内容</param>
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
			new System.Text.RegularExpressions.Regex("\\{([^:\\|\\}]+)(:[^\\|\\}]+)?(\\|[^\\}]+)?\\}");

		/// <summary>
		/// 将字符串中的{Key|格式化字符串|默认值}部分替换为自动中对应的值
		/// </summary>
		/// <param name="tmpl">模板</param>
		/// <param name="args">参数字典</param>
		/// <returns>替换后的字符串</returns>
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
