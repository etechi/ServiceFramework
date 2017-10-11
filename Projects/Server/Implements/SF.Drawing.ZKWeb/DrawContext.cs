using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Maths;

namespace SF.Core.Drawing.dotNetFramework
{
	public class DrawContext : IDrawContext
	{
		Graphics g { get; }
		Bitmap Image { get; }
		public DrawContext(IImageBuffer Buffer)
		{
			this.Buffer = Buffer;
			Image = (Bitmap)((Image)Buffer).RawImage;
			g = Graphics.FromImage(Image);
		}
		public RectD ClipRect
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public Matrix2DD Transform
		{
			get
			{
				var e = g.Transform.Elements;
				return new Matrix2DD(
					e[0], e[1], 0, e[2], e[3], 0, e[4], e[5], 0
					);
			}

			set
			{
				g.Transform = new System.DrawingCore.Drawing2D.Matrix(
					(float)value[0], (float)value[1],
					(float)value[3], (float)value[4],
					(float)value[6], (float)value[7]
					);
			}
		}
		System.DrawingCore.Rectangle FromRectD(RectD rc) =>
				new System.DrawingCore.Rectangle((int)rc.Left, (int)rc.Top, (int)rc.Width, (int)rc.Height);

		public void DrawImage(IImage Image, RectD SourceRect, RectD DestRect)
		{
			g.DrawImage(
				((Image)Image).RawImage,
				FromRectD(DestRect),
				FromRectD(SourceRect),
				GraphicsUnit.Pixel
				);
		}

		public void DrawString(string Text, RectD Rect)
		{
			throw new NotImplementedException();
		}

		public void FillRect(RectD Rect, Color Color)
		{
			using (var b = new SolidBrush(System.DrawingCore.Color.FromArgb(Color.Alpha, Color.Red, Color.Green, Color.Blue)))
				g.FillRectangle(b, (System.DrawingCore.Rectangle)FromRectD(Rect));
		}

		public IImageBuffer Buffer { get; }

		public void Dispose()
		{
			g.Dispose();
		}
	}
}
