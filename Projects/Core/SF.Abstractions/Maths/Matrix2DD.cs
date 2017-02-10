using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Maths
{
	
	public struct Matrix2DD
	{
		public double[] Values { get; }
		public double this[int Index] =>Values[Index];
		public Matrix2DD( double[] Values)
		{
			if (Values == null || Values.Length != 9)
				throw new ArgumentException();
			this.Values = Values;
		}
		public Matrix2DD(
			double v00, double v01, double v02, 
			double v10, double v11, double v12, 
			double v20, double v21, double v22)
		{
			Values = new[]
			{
				v00,v01,v02,
				v10,v11,v21,
				v20,v21,v22
			};
		}
		
		public static Matrix2DD operator * (Matrix2DD x, Matrix2DD y)
		{
			var a = x.Values;
			var b = y.Values;
			return new Matrix2DD(
				a[0] * b[0] + a[1] * b[3] + a[2] * b[6], a[0] * b[1] + a[1] * b[4] + a[2] * b[7], a[0] * b[2] + a[1] * b[5] + a[2] * b[8],
				a[3] * b[0] + a[4] * b[3] + a[5] * b[6], a[3] * b[1] + a[4] * b[4] + a[5] * b[7], a[3] * b[2] + a[4] * b[5] + a[5] * b[8],
				a[6] * b[0] + a[7] * b[3] + a[8] * b[6], a[6] * b[1] + a[7] * b[4] + a[8] * b[7], a[6] * b[2] + a[7] * b[5] + a[8] * b[8]
				);
		}
	}
}
