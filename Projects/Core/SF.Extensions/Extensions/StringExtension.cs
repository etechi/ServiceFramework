using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF
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
			Ensure.NotNull(str,nameof(str));
			return Convert.FromBase64String(str);
		}
        static Random DefaultRandom { get; } = new System.Random();
		public static string Random(this string seed,int length,Random rand=null)
		{
			if (rand == null)
                rand = DefaultRandom;
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
		public static Tuple<string, string> Split2(this string s,char c)
		{
			if (s == null)
				return Tuple.Create((string)null, (string)null);
			var i = s.IndexOf(c);
			if (i == -1)
				return Tuple.Create(s, (string)null);
			return Tuple.Create(s.Substring(0, i), s.Substring(i + 1));
		}
        public static IEnumerable<string> SplitAndNormalizae(this string s,char c=';')
        {
            if (string.IsNullOrEmpty(s))
                return Enumerable.Empty<string>();
            return s.Split(c).Select(i => i.Trim()).Where(i => i.Length > 0).Distinct();
        }
	}
}
