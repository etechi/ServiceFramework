﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Maths
{
	public struct Matrix2DF
	{
		public static Matrix2DF One { get; } = new Matrix2DF(1, 0, 0, 0, 1, 0, 0, 0, 1);
		public static Matrix2DF Zero { get; } = new Matrix2DF(0, 0, 0, 0, 0, 0, 0, 0, 0);
		public float[] Values { get; }
		public float this[int Index] => Values[Index];
		public Matrix2DF(
			float v00, float v01, float v02,
			float v10, float v11, float v12,
			float v20, float v21, float v22)
		{
			Values = new[]
			{
				v00,v01,v02,
				v10,v11,v21,
				v20,v21,v22
			};
		}
	}
}
