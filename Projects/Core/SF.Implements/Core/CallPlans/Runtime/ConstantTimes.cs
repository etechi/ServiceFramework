using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.CallPlans.Runtime
{
	public static class ConstantTimes
	{
		public static readonly DateTime NeverExpire = new DateTime(2200, 1, 1);
		public static readonly DateTime ExecutingStartTime = new DateTime(2100, 1, 1);
		public static readonly DateTime InitTime = new DateTime(2010, 1, 1);

	}
}
