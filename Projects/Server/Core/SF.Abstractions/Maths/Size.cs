using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Maths
{
	public struct Size : IEquatable<Size>
	{
		public Size(int Width,int Height)
		{
			this.Width = Width;
			this.Height = Height;
		}
		public int Width { get; }
		public int Height { get; }

		public override bool Equals(object obj)
		{
			if (obj is Size) return Equals((Size)obj);
			return false;
		}
		public override int GetHashCode()
		{
			return Width.GetHashCode() ^ Height.GetHashCode();
		}
		public override string ToString()
		{
			return $"W:{Width} H:{Height}";
		}
		public bool Equals(Size other)
		{
			return other.Width == Width && other.Height == Height;
		}
		public static bool operator ==(Size x, Size y) =>x.Equals(y);
		public static bool operator !=(Size x, Size y) => !x.Equals(y);

	}

}
