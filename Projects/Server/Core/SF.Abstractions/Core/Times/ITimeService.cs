using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Times
{
	public interface ITimeService
	{
		void Reset();
		void SetTime(DateTime UtcNow);
		DateTime UtcNow { get; }
		DateTime Now { get; }
	}
}
