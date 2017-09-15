using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Maths
{
	public struct RectD
	{
		public RectD(double Left, double Top, double Width, double Height)
		{
			this.Left = Left;
			this.Top = Top;
			this.Width = Width;
			this.Height = Height;
		}
		public double Left { get; }
		public double Top { get; }
		public double Width { get;}
		public double Height { get;}
		public double Right=>Left + Width;
		public double Bottom=>Top + Height; 
	}
	
}
