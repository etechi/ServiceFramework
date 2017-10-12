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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Drawing
{
	public static partial class Transforms
	{
		
		public static Transform<ImageContext> Clip(this Transform<ImageContext> transform, double x, double y, double w, double h)
		{
			return transform.Map(s =>
			{
				var iw = (int)(s.Width * w);
				var ih = (int)(s.Height * h);
				var ix = (int)(s.Width * x);
				var iy = (int)(s.Height * y);

				return new MapPositions
				{
					SrcPosition = new Rectangle(Point.Empty, s),
					DstSize = new Size(iw, ih),
					DstPosition = new Rectangle(ix, iy, iw, ih)
				};
			});
		}
		
		public static Transform<ImageContext> WithLimit(this Transform<ImageContext> transform, int max_width, int max_height)
		{
			return transform.Map(s =>
			{
				int new_width, new_height;
				//限高
				if (max_width == 0)
				{
					if (s.Height <= max_height)
						return null;
					new_height = max_height;
					new_width= s.Width * max_height / s.Height;
					
				}
				//限宽
				else if (max_height == 0)
				{
					if (s.Width <= max_width)
						return null;
					new_height = s.Height * max_width / s.Width;
					new_width = max_width;
				}
				//限高宽
				else if (s.Width <= max_width && s.Height < max_height)
					return null;
				else if (s.Width * max_height > max_width * s.Height)
				{
					new_height = s.Height * max_width / s.Width;
					new_width = max_width;
				}
				else
				{
					new_width = s.Width * max_height / s.Height;
					new_height = max_height;
				}
				return new MapPositions
				{
					SrcPosition = new Rectangle(Point.Empty, s),
					DstSize = new Size(new_width, new_height),
					DstPosition = new Rectangle(0, 0, new_width, new_height)
				};
			});
		}
		public static Transform<ImageContext> Clip(this Transform<ImageContext> transform, int width, int height)
		{
			return transform.Map(s=>
			{
				var x = 0;
				var y = 0;
				var w = s.Width;
				var h = s.Height;
				if (s.Width * height > width * s.Height)
				{
					w = s.Height * width / height;
					x = (s.Width - w) / 2;
				}
				else
				{
					h = s.Width * height / width;
					y = (s.Height - h) / 2;
				}
				return new MapPositions
				{
					SrcPosition = new Rectangle(x, y, w, h),
					DstSize = new Size(width, height),
					DstPosition=new Rectangle(0,0,width,height)
				};
			});
		}
	}
}
