#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Maths
{
	public struct Rectangle : IEquatable<Rectangle>
	{
		public Rectangle(Point Point,Size Size)
		{
			this.Left = Point.X;
			this.Top = Point.Y;
			this.Width = Size.Width;
			this.Height = Size.Height;

		}
		public Rectangle(int Left,int Top,int Width,int Height)
		{
			this.Left = Left;
			this.Top = Top;
			this.Width = Width;
			this.Height = Height;
		}
		public int Left { get;  }
		public int Top { get; }
		public int Width { get;  }
		public int Height { get;}

		public int Right =>Left + Width;
		public int Bottom=>Top + Height;
		public Point Location => new Point(Left, Top);
		public Size Size=>new Size(Width, Height);



		public override bool Equals(object obj)
		{
			if (obj is Rectangle) return Equals((Rectangle)obj);
			return false;
		}
		public override int GetHashCode()
		{
			return Left.GetHashCode() ^ Top.GetHashCode() ^
				Width.GetHashCode() ^ Height.GetHashCode();
		}
		public override string ToString()
		{
			return $"L:{Left} T:{Top} W:{Width} H:{Height}";
		}
		public bool Equals(Rectangle other)
		{
			return other.Left == Left && other.Top == Top &&
				other.Width == Width && other.Height== Height;
		}
		public static bool operator ==(Rectangle x, Rectangle y) => x.Equals(y);
		public static bool operator !=(Rectangle x, Rectangle y) => !x.Equals(y);
	}
}
