using System;
using System.Collections.Generic;
using System.Drawing;
using System.DrawingCore.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Drawing.dotNetFramework
{
	public class Image : IImageBuffer
	{
		public System.DrawingCore.Image RawImage { get; }
		internal Image(System.DrawingCore.Image RawImage)
		{
			this.RawImage = RawImage;
		}
		public IEnumerable<ExifProperty> ExifProperties
		{
			get
			{
				return RawImage.PropertyItems.Select(pi => new ExifProperty
				{
					Id = pi.Id,
					Len = pi.Len,
					Type = pi.Type,
					Value = pi.Value
				});
			}
		}

		public SF.Maths.Size Size
		{
			get
			{
				return new Maths.Size(
					RawImage.Size.Width,
					RawImage.Size.Height
					);
			}
		}

		public string Format
		{
			get
			{
				return Codecs.FindCodecInfo(RawImage.RawFormat.Guid).MimeType;
			}
		}

		public void SaveTo(System.IO.Stream Stream, string Mime, int OutputQuality)
		{
			var ci = Codecs.FindCodecInfo(Mime);
			//png转jpg时需要把透明色改为白色
			EncoderParameters eps = null;
			if (OutputQuality == 0)
				OutputQuality = 95;
			if (OutputQuality > 0)
			{
				eps = new EncoderParameters(1);
				eps.Param[0] = new EncoderParameter(System.DrawingCore.Imaging.Encoder.Quality, OutputQuality);
			}

			RawImage.Save(Stream, ci, eps);
		}

		public void Dispose()
		{
			RawImage.Dispose();
		}
	}
}
