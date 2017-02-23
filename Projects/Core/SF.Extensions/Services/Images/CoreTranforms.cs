﻿using SF.Maths;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Drawing
{
	public class MapPositions
	{
		public Rectangle SrcPosition { get; set; }
		public Rectangle DstPosition { get; set; }
		public Size DstSize { get; set; }
	}

	public static partial class Transforms
	{
		public static Transform<ImageContext> Source { get; } = (src, target) => src;
		public static Transform<ImageContext> Map(this Transform<ImageContext> transform, Func<Size, MapPositions> translate)
		{
			return (src, ctx) =>
			{
				var nimg = ctx.Execute(transform, src);
				var re = translate(nimg.Size);
				if (re == null)
					return nimg;

				if (re.DstSize == re.DstPosition.Size &&
					re.DstPosition.Location == Point.Empty &&
					re.SrcPosition.Location == Point.Empty &&
					re.SrcPosition.Size == nimg.Size &&
					re.DstSize == nimg.Size
					)
					return nimg;


				var img = nimg.Image;
				var dst = ctx.GetTarget(re.DstSize);
				using (var g = ctx.NewGraphics(dst))
				{
					if (re.DstSize != re.DstPosition.Size)
						g.FillRect(new Rectangle(Point.Empty, re.DstSize).ToRectD(), Color.White);
					g.DrawImage(
						img,
						re.SrcPosition.ToRectD(),
						re.DstPosition.ToRectD()
						);
				}
				return new ImageContext { Image = dst, Size = re.DstSize };
			};
		}
		public static Transform<ImageContext> Filter(this Transform<ImageContext> transform, Transform<ImageContext> filter)
		{
			return (img, ctx) =>
			{
				var re=ctx.Execute(transform, img, ctx.Flags);
				filter(re, ctx);
				return re;
			};
		}
		
		public static Transform<ImageContext> SaveTo(this Transform<ImageContext> src, System.IO.Stream dst, string mime = null, int outputQuality = 0)
		{
			return (img, ctx) =>
			{
				var rawFormat = img.Image.Format;
				img = ctx.Execute(src, img, ExecuteFlag.StrictSize);
				img.Image.SaveTo(dst, mime?? rawFormat, outputQuality);
				return img;
			};
		}
		public static Transform<ImageMemoryBuffer> ToMemoryBuffer(this Transform<ImageContext> src, string mime = null, int outputQuality = 0)
		{
			return (img, ctx) =>
			{
				var rawFormat = img.Image.Format;
				img = ctx.Execute(src, img, ExecuteFlag.StrictSize);
				var ms = new System.IO.MemoryStream();
				mime= mime ?? rawFormat;
				img.Image.SaveTo(ms, mime, outputQuality);
				return new ImageMemoryBuffer
				{
					Data = ms.ToArray(),
					Size = img.Image.Size,
					Mime = mime
				};
			};
		}
		public static Transform<byte[]> ToByteArray(this Transform<ImageContext> src, string mime = null, int outputQuality = 0)
		{
			return (img, ctx) =>
				ToMemoryBuffer(src, mime, outputQuality)(img, ctx).Data;
		}
		public static T ApplyTo<T>(this Transform<T> transform, IImage image, IImageProvider Provider)
		{
			using (var ctx = new TransformContext(Provider))
			{
				return ctx.Execute(
					transform,
					new ImageContext { Image = image, Size = image.Size },
					ExecuteFlag.StrictSize
					);
			}
		}
		public static T ApplyTo<T>(this Transform<T> transform, System.IO.Stream stream, IImageProvider Provider)
		{
			using(var img = Provider.Load(stream))
			{
				return transform.ApplyTo(img, Provider);
			}
		}
		public static T ApplyTo<T>(this Transform<T> transform, Func<System.IO.Stream> stream, IImageProvider Provider)
		{
			using (var s = stream())
				return transform.ApplyTo(s, Provider);
		}
		public static async Task<T> ApplyTo<T>(this Transform<T> transform, Func<Task<System.IO.Stream>> stream, IImageProvider Provider)
		{
			using (var s = await stream())
				return transform.ApplyTo(s, Provider);
		}
		public static T ApplyTo<T>(this Transform<T> transform,byte[] byteArray, IImageProvider Provider)
		{
			return transform.ApplyTo(new System.IO.MemoryStream(byteArray), Provider);
		}

	}
}
