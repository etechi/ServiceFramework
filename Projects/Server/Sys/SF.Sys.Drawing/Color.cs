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

using SF.Maths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Drawing
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
