using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Maths
{
	public static class RectExtensions

	{
		public static RectD ToRectD(this Rectangle rc) =>
			new RectD(rc.Left, rc.Top, rc.Width, rc.Height);
	}
}
