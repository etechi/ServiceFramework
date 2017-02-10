using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Maths
{
	public struct Point:IEquatable<Point>
	{
		public static Point Empty { get; } = new Point(0, 0);

		public Point(int X,int Y)
		{
			this.X = X;
			this.Y = Y;
		}
		public int X { get; }
		public int Y { get; }


		public override bool Equals(object obj)
		{
			if (obj is Size) return Equals((Size)obj);
			return false;
		}
		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}
		public override string ToString()
		{
			return $"X:{X} Y:{Y}";
		}
		public bool Equals(Point other)
		{
			return other.X == X && other.Y == Y;
		}
		public static bool operator ==(Point x, Point y) => x.Equals(y);
		public static bool operator !=(Point x, Point y) => !x.Equals(y);
	}
	
}
