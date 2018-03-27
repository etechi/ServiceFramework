using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF.Sys
{
	public static class DateTimeUtils
	{
		public static DateTime JSDateOffset { get; } = new DateTime(1970, 1, 1,0,0,0,DateTimeKind.Local).AddHours(8);
		/// <summary>
		/// 转换成JS格式时间，当前时间必须是UTC格式
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static long ToJsTime(this DateTime time)
		{
			return (long)time.Subtract(JSDateOffset).TotalMilliseconds;
		}
        public static uint ToUnixTime(this DateTime time)
        {
            return (uint)(time.ToJsTime() / 1000);
        }
		public static DateTime FromJsTime(long time)
		{
			return JSDateOffset.AddMilliseconds(time);
		}
		public static DateTime FromUnixTime(long time)
		{
			return JSDateOffset.AddMilliseconds(time*1000);
		}
		public static DateTime FloorToSecond(this DateTime Time)
		{
			return new DateTime((Time.Ticks / 10000000) * 10000000);
		}
		public static DateTime ToUtcTime(this DateTime Time)
		{
			switch (Time.Kind)
			{
				case DateTimeKind.Local:
					return Time.ToUniversalTime();
				case DateTimeKind.Unspecified:
					return new DateTime(Time.Ticks, DateTimeKind.Local).ToUniversalTime();
				case DateTimeKind.Utc:
					return Time;
				default:
					throw new NotSupportedException();
			}
		}
	}
}
