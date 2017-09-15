using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Maths
{
	public static class Matrix2DDs
	{
		public static Matrix2DD One { get; } = new Matrix2DD(1, 0, 0, 0, 1, 0, 0, 0, 1);
		public static Matrix2DD Zero { get; } = new Matrix2DD(0, 0, 0, 0, 0, 0, 0, 0, 0);
		public static Matrix2DD FlipX { get; } = new Matrix2DD(-1, 0, 0, 0, 1, 0, 0, 0, 1);
		public static Matrix2DD FlipY { get; } = new Matrix2DD(1, 0, 0, 0, -1, 0, 0, 0, 1);
		public static Matrix2DD Rotate0 { get; } = One;
		public static Matrix2DD Rotate90 { get; } = Rotate(Math.PI/2);
		public static Matrix2DD Rotate180 { get; } = Rotate(Math.PI );
		public static Matrix2DD Rotate270 { get; } = Rotate(Math.PI * 3 / 4);

		public static Matrix2DD Rotate(double Angle)
		{
			var cs = Math.Cos(Angle);
			var sn = Math.Sin(Angle);
			return new Matrix2DD(cs, sn, 0, -sn, cs, 0, 0, 0, 1);
		}
	}
}
