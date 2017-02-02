using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF
{
	public static class NumberExtension
	{
		public static string Hex(this int value)
		{
			return value.ToString("x");
		}
		public static string Hex(this long value)
		{
			return value.ToString("x");
		}
		public static long HexNumber(this string value)
		{
			return long.Parse(value, System.Globalization.NumberStyles.HexNumber, null);
		}
		public static int Int32(this long value)
		{
			return (int)value;
		}

        public static bool IsZero(this double value, double threshold = 1e-10)
            => Math.Abs(value) < threshold;

        public static bool IsZero(this float value, float threshold = 1e-10f)
            => Math.Abs(value) < threshold;

        public static int Round(this double value)
            => (int)(Math.Round(value)+0.1);

        public static int Round(this float value)
            => (int)(Math.Round(value) + 0.1);

        public static int Round(this decimal value)
           => (int)(Math.Round(value) + 0.1m);

        public static int Floor(this double value)
            => (int)(Math.Floor(value) + 0.1);

        public static int Floor(this float value)
            => (int)(Math.Floor(value) + 0.1);

        public static int Floor(this decimal value)
           => (int)(Math.Floor(value) + 0.1m);
    }
}
