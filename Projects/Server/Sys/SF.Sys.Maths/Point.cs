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
