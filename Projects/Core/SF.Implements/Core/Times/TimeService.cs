using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Times
{
	public class TimeService : ITimeService
	{
		TimeSpan Offset;
		public DateTime Now
		{
			get
			{
				return DateTime.Now.Add(Offset);
			}
		}

		public DateTime UtcNow
		{
			get
			{
				return DateTime.UtcNow.Add(Offset);
			}
		}

		public void Reset()
		{
			Offset = TimeSpan.Zero;
		}

		public void SetTime(DateTime UtcNow)
		{
			Offset = UtcNow.Subtract(DateTime.UtcNow);
		}
	}
}
