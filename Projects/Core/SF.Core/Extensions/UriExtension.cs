using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF
{
	public static class UriExtension
	{
        public static Uri WithQueryString(this Uri uri, params Tuple<string, string>[] args) =>
            uri.WithQueryString(args.Select(a => new KeyValuePair<string, string>(a.Item1, a.Item2)).ToArray());

        public static Uri WithQueryString<T>(this Uri uri, params KeyValuePair<string,T>[] args)
		{
			var qs = EncodeQueryString<T>(args);
			var b = new UriBuilder(uri);
			b.Query = qs;
			return b.Uri;
		}
		public static Uri WithFragment(this Uri uri, string fragment)
		{
			var b = new UriBuilder(uri);
			b.Fragment = fragment;
			return b.Uri;
		}
		public static IEnumerable<KeyValuePair<string,string>> ParseQuery(this Uri uri)
		{
			var q = uri.Query;
			if (string.IsNullOrWhiteSpace(q)) return Enumerable.Empty<KeyValuePair<string, string>>();
			var offset = q[0] == '?' ? 1 : 0;
			return DecodeQueryString(q, offset);
		}
        private class UrlDecoder
        {
            private int _bufferSize;

            private int _numChars;

            private char[] _charBuffer;

            private int _numBytes;

            private byte[] _byteBuffer;

            private Encoding _encoding;

            private void FlushBytes()
            {
                if (this._numBytes > 0)
                {
                    this._numChars += this._encoding.GetChars(this._byteBuffer, 0, this._numBytes, this._charBuffer, this._numChars);
                    this._numBytes = 0;
                }
            }

            internal UrlDecoder(int bufferSize, Encoding encoding)
            {
                this._bufferSize = bufferSize;
                this._encoding = encoding;
                this._charBuffer = new char[bufferSize];
            }

            internal void AddChar(char ch)
            {
                if (this._numBytes > 0)
                {
                    this.FlushBytes();
                }
                char[] arg_27_0 = this._charBuffer;
                int numChars = this._numChars;
                this._numChars = numChars + 1;
                arg_27_0[numChars] = ch;
            }

            internal void AddByte(byte b)
            {
                if (this._byteBuffer == null)
                {
                    this._byteBuffer = new byte[this._bufferSize];
                }
                byte[] arg_31_0 = this._byteBuffer;
                int numBytes = this._numBytes;
                this._numBytes = numBytes + 1;
                arg_31_0[numBytes] = b;
            }

            internal string GetString()
            {
                if (this._numBytes > 0)
                {
                    this.FlushBytes();
                }
                if (this._numChars > 0)
                {
                    return new string(this._charBuffer, 0, this._numChars);
                }
                return string.Empty;
            }
        }
        static int HexToInt(char h)
        {
            if (h >= '0' && h <= '9')
            {
                return (int)(h - '0');
            }
            if (h >= 'a' && h <= 'f')
            {
                return (int)(h - 'a' + '\n');
            }
            if (h < 'A' || h > 'F')
            {
                return -1;
            }
            return (int)(h - 'A' + '\n');
        }
        static string ValidateString(string input, bool skipUtf16Validation)
        {
            if (skipUtf16Validation || string.IsNullOrEmpty(input))
            {
                return input;
            }
            int num = -1;
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsSurrogate(input[i]))
                {
                    num = i;
                    break;
                }
            }
            if (num < 0)
            {
                return input;
            }
            char[] array = input.ToCharArray();
            for (int j = num; j < array.Length; j++)
            {
                char c = array[j];
                if (char.IsLowSurrogate(c))
                {
                    array[j] = '?';
                }
                else if (char.IsHighSurrogate(c))
                {
                    if (j + 1 < array.Length && char.IsLowSurrogate(array[j + 1]))
                    {
                        j++;
                    }
                    else
                    {
                        array[j] = '?';
                    }
                }
            }
            return new string(array);
        }
        public static string UrlDecode(string value, Encoding encoding)
        {
            if (value == null)
            {
                return null;
            }
            int length = value.Length;
            UrlDecoder urlDecoder = new UrlDecoder(length, encoding);
            int i = 0;
            while (i < length)
            {
                char c = value[i];
                if (c == '+')
                {
                    c = ' ';
                    goto IL_10B;
                }
                if (c != '%' || i >= length - 2)
                {
                    goto IL_10B;
                }
                if (value[i + 1] == 'u' && i < length - 5)
                {
                    int num = HexToInt(value[i + 2]);
                    int num2 = HexToInt(value[i + 3]);
                    int num3 = HexToInt(value[i + 4]);
                    int num4 = HexToInt(value[i + 5]);
                    if (num < 0 || num2 < 0 || num3 < 0 || num4 < 0)
                    {
                        goto IL_10B;
                    }
                    c = (char)(num << 12 | num2 << 8 | num3 << 4 | num4);
                    i += 5;
                    urlDecoder.AddChar(c);
                }
                else
                {
                    int num5 = HexToInt(value[i + 1]);
                    int num6 = HexToInt(value[i + 2]);
                    if (num5 < 0 || num6 < 0)
                    {
                        goto IL_10B;
                    }
                    byte b = (byte)(num5 << 4 | num6);
                    i += 2;
                    urlDecoder.AddByte(b);
                }
                IL_125:
                i++;
                continue;
                IL_10B:
                if ((c & 'ﾀ') == '\0')
                {
                    urlDecoder.AddByte((byte)c);
                    goto IL_125;
                }
                urlDecoder.AddChar(c);
                goto IL_125;
            }
            return ValidateString(urlDecoder.GetString(), false);
        }

        public static IEnumerable<KeyValuePair<string, string>> DecodeQueryString(string str, int offset = 0, int size = -1,Encoding Encoding=null)
		{
			if (str == null)
				yield break;
			if (size == -1)
				size = str.Length - offset;
			if (offset < 0 || size < 0 || offset + size > str.Length)
				throw new ArgumentOutOfRangeException("offset");
			int vs;
			int ve;
			int i = offset;
			var end = offset + size;
            
			for (;;)
			{
				ve = str.IndexOf('&', i, end - i);
				var len = ve == -1 ? end - i : ve - i;
				vs = str.IndexOf('=', i, len);
				var key = str.Substring(i, vs == -1 ? len : vs - i);
				var value = vs == -1 ? null : str.Substring(vs + 1, len - (vs + 1 - i));
                var keyDecoded = Uri.UnescapeDataString(key);
                if (value == null)
                    yield return new KeyValuePair<string, string>(keyDecoded, string.Empty);
                else if (Encoding == null || Encoding == Encoding.UTF8)
                    yield return new KeyValuePair<string, string>(keyDecoded, Uri.UnescapeDataString(value));
                else
                    yield return new KeyValuePair<string, string>(keyDecoded, UrlDecode(value, Encoding));
				if (ve == -1)
					break;
				i = ve + 1;
			}
		}
        public static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char)(n + 48);
            }
            return (char)(n - 10 + 97);
        }
        public static bool IsUrlSafeChar(char ch)
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
            {
                return true;
            }
            if (ch != '!')
            {
                switch (ch)
                {
                    case '(':
                    case ')':
                    case '*':
                    case '-':
                    case '.':
                        return true;
                    case '+':
                    case ',':
                        break;
                    default:
                        if (ch == '_')
                        {
                            return true;
                        }
                        break;
                }
                return false;
            }
            return true;
        }
        static string UrlEncodeWithEncoding(string value,Encoding encoding)
        {
            var bytes = encoding.GetBytes(value);
            int sp_count = 0;
            int unsafe_count = 0;
            var offset = 0;
            var count = bytes.Length;
            for (int i = 0; i < count; i++)
            {
                char c = (char)bytes[offset + i];
                if (c == ' ')
                {
                    sp_count++;
                }
                else if (!IsUrlSafeChar(c))
                {
                    unsafe_count++;
                }
            }
            if (sp_count == 0 && unsafe_count == 0)
                return value;
            byte[] array = new byte[count + unsafe_count * 2];
            int num3 = 0;
            for (int j = 0; j < count; j++)
            {
                byte b = bytes[offset + j];
                char c2 = (char)b;
                if (IsUrlSafeChar(c2))
                {
                    array[num3++] = b;
                }
                else if (c2 == ' ')
                {
                    array[num3++] = 43;
                }
                else
                {
                    array[num3++] = 37;
                    array[num3++] = (byte)IntToHex(b >> 4 & 15);
                    array[num3++] = (byte)IntToHex((int)(b & 15));
                }
            }
            return Encoding.ASCII.GetString(array);
        }
        static void EscapeDataString(StringBuilder sb, string str, Encoding e)
		{
			if (e == null || e==Encoding.UTF8)
			{
				sb.Append(Uri.EscapeDataString(str));
				return;
			}
            sb.Append(UrlEncodeWithEncoding(str, e));
			//var b = e.GetBytes(str);
			//foreach (var c in b)
			//{
			//	char cc = (char)c;
			//	if (cc >= 'a' && cc <= 'z' ||
			//		cc >= 'A' && cc <= 'Z' ||
			//		cc >= '0' && cc <= '9' ||
			//		cc == '_' ||
			//		cc == '-' ||
			//		cc == '*')
			//		sb.Append(cc);
			//	else
			//		sb.AppendFormat("%{0:x02}", c);
			//}
		}
        public static string EncodeQueryString(
            params Tuple<string,string>[] items
            )
        {
            return EncodeQueryString<string>(
                items.Select(it => new KeyValuePair<string, string>(it.Item1, it.Item2))
                );
        }

        public static string EncodeQueryString<T>(
			IEnumerable<KeyValuePair<string, T>> pairs,
			Encoding encoding = null
			)
		{
			var sb = new StringBuilder();
			var first = true;
			foreach (var de in pairs)
			{
				if (first) first = false;
				else sb.Append('&');

				EscapeDataString(sb, de.Key, encoding);
				sb.Append('=');
				if (de.Value == null)
					continue;
				var v = de.Value.ToString();
				if (v.Length > 0)
					EscapeDataString(sb, v, encoding);
			}
			return sb.ToString();
		}
        public static async Task<byte[]> GetBytes(this Uri uri)
        {
            using (var cli = new HttpClient())
            {
                return await cli.GetByteArrayAsync(uri);
            }
        }
		static UriExtension()
		{
			//Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			GBK = Encoding.GetEncoding("GBK");
		}
		public static Encoding GBK { get; }
        public static async Task<string> GetString(this Uri uri,Encoding Encoding=null, int Retry=5,int Timeout = 20)
        {
			for (var i=0;i<Retry;i++)
			{
				try
				{
					using (var cli = new HttpClient())
					{
						cli.Timeout = TimeSpan.FromSeconds(Timeout);
						return Encoding == null ?
							await cli.GetStringAsync(uri) :
							Encoding.GetString(await cli.GetByteArrayAsync(uri))
							;
					}
				}
				catch
				{
					if (i == Retry - 1)
						throw;
				}
			}
			throw new NotSupportedException();
        }
		public static async Task<T> Get<T>(this Uri uri,Encoding Encoding = null, int Retry = 5, int Timeout = 20)
		{
			var str = await uri.GetString( Encoding, Retry, Timeout);
			return Json.Parse<T>(str);
		}
        public static async Task<string> PostAndReturnString(this Uri uri, HttpContent content)
        {
            using (var cli = new HttpClient())
            {
                cli.Timeout = TimeSpan.FromSeconds(10);
                using (var re = await cli.PostAsync(uri, content))
                {
                    return await re.Content.ReadAsStringAsync();
                }
            }
        }
        public static Task<string> PostAndReturnString(this Uri uri,params Tuple<string,string>[] args)
        {
            return PostAndReturnString(uri, new StringContent(
                         UriExtension.EncodeQueryString(args),
                         Encoding.UTF8,
                         "application/x-www-form-urlencoded"
                         ));
        }
    }
}
