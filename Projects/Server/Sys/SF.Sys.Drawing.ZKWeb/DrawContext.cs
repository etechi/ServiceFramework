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
using System.DrawingCore;
using SF.Maths;

namespace SF.Sys.Drawing.dotNetFramework
{
	public class Brush : IBrush,IDisposable
	{
		public System.DrawingCore.Brush SysBrush { get;  }
		public Brush(System.DrawingCore.Brush SysBrush)
		{
			this.SysBrush = SysBrush;
		}
		public void Dispose()
		{
			SysBrush.Dispose();
		}
	}
	public class Font : IFont,IDisposable {
		public System.DrawingCore.Font SysFont{ get; set; }
		public Font(System.DrawingCore.Font SysFont)
		{
			this.SysFont = SysFont;
		}
		public void Dispose()
		{
			SysFont.Dispose();
		}
	}

	public class DrawContext : IDrawContext
	{
		Graphics g { get; }
		Bitmap Image { get; }
		IResourceCache Cache { get; }
		public DrawContext(IImageBuffer Buffer, IResourceCache Cache)
		{
			this.Buffer = Buffer;
			this.Cache = Cache;
			Image = (Bitmap)((Image)Buffer).RawImage;
			g = Graphics.FromImage(Image);
			g.TextRenderingHint = System.DrawingCore.Text.TextRenderingHint.AntiAlias;
			
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
		public void ResetTransform()
		{
			g.ResetTransform();
		}
		public void TranslateTransform(float x, float y)
		{
			g.TranslateTransform(x, y);
		}
		public void RotateTransform(float deg)
		{ 
			g.RotateTransform(deg);
		}
		public void ScaleTransform(float sx, float sy)
		{
			g.ScaleTransform(sx,sy);
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

		public void Clear(Color color)
		{
			g.Clear(ToSysColor(color));
		}
		public void DrawImage(IImage Image, RectD SourceRect, RectD DestRect)
		{
			g.DrawImage(
				((Image)Image).RawImage,
				FromRectD(DestRect),
				FromRectD(SourceRect),
				GraphicsUnit.Pixel
				);
		}

		public void DrawString(string text, RectD rect, IFont font,IBrush brush)
		{
			g.DrawString(text, ((Font)font).SysFont, ((Brush)brush).SysBrush, FromRectD(rect));
		}
		public IFont GetFont(string fontFamily,int fontSize, FontStyle fontStyle)
		{
			var key = "font:" + fontFamily+"/"+fontSize+"/"+fontSize;
			var f = (Font)Cache.GetResource(key);
			if (f == null)
				Cache.SetResource(key, f = new Font(new System.DrawingCore.Font(fontFamily,fontSize, (System.DrawingCore.FontStyle)fontStyle)));
			return f;
		}
		static System.DrawingCore.Color ToSysColor(Color color)
			=> System.DrawingCore.Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);

		public IBrush GetSolidBrush(Color Color)
		{
			var key = "brush:" + Color.ToString();
			var b = (Brush)Cache.GetResource(key);
			if (b == null)
				Cache.SetResource(key, b =new Brush(new SolidBrush(System.DrawingCore.Color.FromArgb(Color.Alpha, Color.Red, Color.Green, Color.Blue))));
			return b;
		}
		public void FillRect(RectD Rect, IBrush brush)
		{
			g.FillRectangle(((Brush)brush).SysBrush, FromRectD(Rect));
		}

		public IImageBuffer Buffer { get; }

		public void Dispose()
		{
			g.Dispose();
		}
	}
}
