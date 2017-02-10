using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Drawing
{
	public static partial class Transforms
	{

		public static Transform<ImageContext>  ExifOrientationProcess(this Transform<ImageContext> transform, bool skip=false)
		{
			if (skip)
				return transform;
			return (img, ctx) =>
			{
				throw new NotSupportedException();
				//img = ctx.Execute(transform, img);
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
