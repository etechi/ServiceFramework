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
	public interface IBrush{ }
	public interface IFont{ }
	public interface IDrawContext : IDisposable
    {
		IImageBuffer Buffer { get; }
		Maths.Matrix2DD Transform { get; set; }
		Maths.RectD ClipRect { get; set; }
		IBrush GetSolidBrush(Color color);
		IFont GetFont(string FontFamily, int FontSize, FontStyle fontStyle);
		void Clear(Color color);
		void DrawImage(IImage Image, RectD SourceRect, RectD DestRect);
		void DrawString(string Text, RectD Rect, IFont Font, IBrush Brush);
		void FillRect(RectD Rect, IBrush Brush);

		void ResetTransform();
		void TranslateTransform(float x, float y);
		void RotateTransform(float deg);
		void ScaleTransform(float sx, float sy);
	}
	public static class DrawContextExtensions
	{
		public static void FillRect(this IDrawContext Context, RectD Rect, Color color)
			=> Context.FillRect(Rect, Context.GetSolidBrush(color));
		public static void FillRect(this IDrawContext Context, RectD Rect, string color)
			=> Context.FillRect(Rect, Color.Parse(color));

		public static void Clear(this IDrawContext Context, string color)
			=> Context.Clear(Color.Parse(color));

	}
}
