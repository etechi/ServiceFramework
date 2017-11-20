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
