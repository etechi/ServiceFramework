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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Drawing
{
	public static partial class Transforms
	{

		public static Transform<ImageContext>  ExifOrientationProcess(this Transform<ImageContext> transform, bool skip=false)
		{
			if (skip)
				return transform;
			return (img, ctx) =>
			{
				
				img = ctx.Execute(transform, img);
				return img;
				//var pi = img.Image.ExifProperties.FirstOrDefault(i => i.Id == 274);
				//if (pi == null)
				//	return img;

				//var dst = ctx.GetTarget(re.DstSize);
				//using (var g = ctx.NewGraphics(dst))
				//{
				//	if (re.DstSize != re.DstPosition.Size)
				//		g.FillRect(new Rectangle(Point.Empty, re.DstSize).ToRectD(), Color.White);
				//	g.DrawImage(
				//		img,
				//		re.DstPosition.ToRectD(),
				//		re.SrcPosition.ToRectD()
				//		);
				//}
				//return new ImageContext { Image = dst, Size = re.DstSize };



				//switch (pi.Value[0])
				//{
				//	case 2:
				//		img.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
				//		break;
				//	case 3:
				//		img.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
				//		break;
				//	case 4:
				//		img.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
				//		break;
				//	case 5:
				//		img.Image.RotateFlip(RotateFlipType.Rotate90FlipY);
				//		break;
				//	case 6:
				//		img.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
				//		break;
				//	case 7:
				//		img.Image.RotateFlip(RotateFlipType.Rotate270FlipY);
				//		break;
				//	case 8:
				//		img.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
				//		break;
				//}
				//return img;
			};
		}
	}
}
