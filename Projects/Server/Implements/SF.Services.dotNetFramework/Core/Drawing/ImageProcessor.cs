using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Maths;

namespace SF.Core.Drawing.dotNetFramework
{
	public class ImageProcessor : IImageProvider
	{
		public IImage Load(Stream Stream)
		{
			return new Image(
				Bitmap.FromStream(Stream)
				);
		}

	
		public IDrawContext NewDrawContext(IImageBuffer Image)
		{
			return new DrawContext(Image);

		}

		public IImageBuffer NewImageBuffer(Maths.Size Size)
		{
			return new Image(
				new Bitmap(Size.Width, Size.Height)
				);
		}
	}
}
