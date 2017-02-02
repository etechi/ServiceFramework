using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SF
{
	public static class ExceptionExtension
	{
		public static string GetInnerExceptionMessage(this Exception e)
		{
			if (e == null)
				return string.Empty;
			e = e.Raduce(i => i.InnerException != null, i => i.InnerException);
			return e.Message;
		}
	}
}
