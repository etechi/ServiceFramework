using SF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace System
{
	public static class BytesExtension
	{
		static readonly char[] hex_chars = {
			'0', '1', '2', '3',
			'4', '5', '6', '7',
			'8', '9', 'a', 'b',
			'c', 'd', 'e', 'f'
		};
		static readonly char[] hex_upper_chars = {
			'0', '1', '2', '3',
			'4', '5', '6', '7',
			'8', '9', 'A', 'B',
			'C', 'D', 'E', 'F'
		};
        public static byte[] GZip(this byte[] data)
        {
            return GZip(data, 0, data.Length);
        }
        public static byte[] GZip(this byte[] data,int offset,int length)
        {
            using(var ms=new System.IO.MemoryStream(data.Length))
            {
                using (var gs = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress))
                {
                    gs.Write(data, offset, length);
                }
                return ms.ToArray();
            }
        }
        public static byte[] GUnzip(this byte[] data)
        {
            return GUnzip(data, 0, data.Length);
        }
        public static byte[] GUnzip(this byte[] data, int offset, int length)
        {
            using (var rs = new System.IO.MemoryStream(length * 3))
            {
                using (var ms = new System.IO.MemoryStream(data, offset, length))
                using (var gs = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress))
                    gs.CopyTo(rs);
                return rs.ToArray();
            }
        }
        public static string Hex(this byte[] data,bool upper=false){
			Ensure.NotNull(data,nameof(data));

			var len = data.Length;
			var buf = new char[len * 2];
			var chars = upper ? hex_upper_chars : hex_chars;
			for(var i=0;i<len;i++)
			{
				var v = data[i];
				buf[i * 2] = chars[v >> 4];
				buf[i * 2+1] = chars[v & 0xf];
			}
			return new string(buf);
			
		}
		public static string Base64(this byte[] data)
		{
			Ensure.NotNull(data,nameof(data));
			return Convert.ToBase64String(data);
		}
		public static string UTF8String(this byte[] data)
		{
			if (data == null) return string.Empty;
			return Encoding.UTF8.GetString(data);
		}
		static unsafe int memcmp(byte* buf1, byte* buf2, int count)
		{
			if (count == 0)
				return 0;
			while ((--count) > 0 && *buf1 == *buf2)
			{
				buf1++;
				buf2++;
			}
			return (int)*buf1 - (int)*buf2;
		}
		public static int Compare(this byte[] a1, byte[] a2)
		{
			return Compare(a1, 0, a1.Length, a2, 0, a2.Length);
		}
		public unsafe static int Compare(this byte[] a1, int o1, int l1, byte[] a2, int o2, int l2)
		{
			int e1 = o1 + l1;
			if (a1 == null || a1.Length < e1) throw new ArgumentException("compare", "a1");
			int e2 = o2 + l2;
			if (a2 == null || a2.Length < e2) throw new ArgumentException("compare", "a2");

			int l = l1 < l2 ? l1 : l2;
			int re = 0;
			fixed (byte* p1 = &a1[o1])
			{
				fixed (byte* p2 = &a2[o2])
				{
					re = memcmp(p1, p2, l);
				}
			}

			if (re != 0)
				return re;
			if (l1 > l2) return 1;
			else if (l1 < l2) return -1;
			return 0;
		}
		static unsafe byte* memchr(byte* p, int size, byte ch)
		{
			var pe = p + size;
			while (p < pe && (*p != ch))
				p++;
			return p == pe ? null : p;
		}
		public static unsafe int IndexOf(this byte[] buf, byte ch)
		{
			return buf.IndexOf(0, buf.Length, ch);
		}
		public static unsafe int IndexOf(this byte[] buf, int offset, byte ch)
		{
			return buf.IndexOf(offset, buf.Length - offset, ch);
		}
		public static unsafe int IndexOf(this byte[] buf, int off, int size, byte ch)
		{
			if (buf == null)
				throw new ArgumentNullException("buf");
			int len = buf.Length;
			int end = off + size;
			if (len < end)
				throw new ArgumentOutOfRangeException("off");
			if (len < end || size == 0)
				return -1;
			fixed (byte* p = &buf[off])
			{
				var tp = memchr(p, size, ch);
				return tp == null ? -1 : (int)(tp - p) + off;
			}
		}
		public unsafe static int IndexOf(this byte[] s1, int o1, int l1, byte[] s2, int o2, int l2)
		{
			if (s1 == null || o1 + l1 > s1.Length) throw new ArgumentException("index_of", "s1");
			if (s2 == null || o2 + l2 > s2.Length) throw new ArgumentException("index_of", "s2");
			if (l2 > l1) return -1;
			byte first = s2[o2];

			int s = o1;
			fixed (byte* p1 = &s1[o1])
			{
				if (l2 == 1)
				{
					byte* re = memchr(p1, (int)(uint)first, (byte)l1);
					return re == null ? -1 : (int)(re - p1) + o1;
				}

				l2--;
				byte* p = p1;
				byte* e = p + l1 - l2 + 1;
				fixed (byte* p2 = &s2[o2 + 1])
				{
					for (;;)
					{
						byte* t = memchr(p, (int)(e - p), first);
						if (t == null)
							return -1;

						if (memcmp(t + 1, p2, l2) == 0)
							return (int)(t - p1) + o1;
						p = t + 1;
						if (p >= e)
							return -1;
					}
				}
			}
		}

		public static ushort ReadUInt16BE(this byte[] buf, int offset)
		{
			return (ushort)((buf[offset] << 8) | buf[offset + 1]);
		}
		public static void WriteUInt16BE(this byte[] buf, int offset, ushort value)
		{
			buf[offset] = (byte)(value >> 8);
			buf[offset + 1] = (byte)value;
		}
		public static int ReadInt32BE(this byte[] buf, int offset)
		{
			return (int)((buf[offset] << 24) | (buf[offset+1] << 16) | (buf[offset+2] << 8) | buf[offset + 3]);
		}
		public static void WriteInt32BE(this byte[] buf, int offset, int value)
		{
			buf[offset] = (byte)(value >> 24);
			buf[offset+1] = (byte)(value >> 16);
			buf[offset+2] = (byte)(value >> 8);
			buf[offset + 3] = (byte)value;
		}
	}
}
