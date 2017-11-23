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

using System.Collections.Generic;
using System.DrawingCore.Imaging;
using System.Linq;

namespace SF.Sys.Drawing.dotNetFramework
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
