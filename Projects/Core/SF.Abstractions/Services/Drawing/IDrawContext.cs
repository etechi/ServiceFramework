using SF.Maths;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Drawing
{
	public interface IDrawContext : IDisposable
    {
		IImageBuffer Buffer { get; }
		Maths.Matrix2DD Transform { get; set; }
		Maths.RectD ClipRect { get; set; }
		void DrawImage(IImage Image, RectD SourceRect, RectD DestRect);
		void DrawString(string Text, RectD Rect);
		void FillRect(RectD Rect, Color Color);
	}
}
