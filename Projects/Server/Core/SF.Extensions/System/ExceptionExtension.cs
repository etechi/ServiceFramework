using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
	public static class ExceptionExtension
	{
		public static Exception GetRootException(this Exception e)
		{
			if (e == null)
				return null;
			while (e.InnerException != null)
				e = e.InnerException;
			return e;
		}
	}
}
