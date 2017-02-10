using SF.Maths;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Drawing
{
	public struct Color
    {
		public static Color Black { get; } = new Color(0,0,0,255);
		public static Color White { get; } = new Color(255, 255, 255, 255);
		public static Color Transparent { get; } = new Color(0, 0, 0, 0);
		public Color(int R,int G,int B,int A)
		{
			Value =
				(((uint)A & 0xff) << 24) |
				(((uint)R & 0xff) << 16) |
				(((uint)G & 0xff) << 8) |
				(((uint)B & 0xff) << 0);
		}

		public uint Value { get; set; }
		public int Red { get { return 0xff & (int)(Value >> 16); } set { Value = (Value & 0xff00ffff) | ((Value & 0xff) << 16); } }
		public int Green { get { return 0xff & (int)(Value >> 8); } set { Value = (Value & 0xffff00ff) | ((Value & 0xff) << 8); } }
		public int Blue { get { return 0xff & (int)(Value >> 0); } set { Value = (Value & 0xffffff00) | ((Value & 0xff) << 0); } }
		public int Alpha { get { return 0xff & (int)(Value >> 24); } set { Value = (Value & 0x00ffffff) | ((Value & 0xff) << 24); } }
	}
}
