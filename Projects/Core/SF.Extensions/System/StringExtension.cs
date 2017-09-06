using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
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

		public static byte[] Base64(this string str)
		{
			SF.Ensure.NotNull(str,nameof(str));
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
		public static DateTime ToDateTime(this string str, string Format, System.Globalization.DateTimeStyles Styles = Globalization.DateTimeStyles.AssumeLocal)
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
		public static DateTime? TryToDateTime(this string str, string Format, System.Globalization.DateTimeStyles Styles = Globalization.DateTimeStyles.AssumeLocal)
					=> DateTime.TryParseExact(str, Format, null, Styles,out var re)?(DateTime?)re:null;

	}
}
