using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.DrawingCore.Drawing2D;

namespace SF.Core.Drawing.dotNetFramework
{

	public static class Codecs
	{
		static Dictionary<string, ImageCodecInfo> _infos = new Dictionary<string, ImageCodecInfo>();
		static Codecs()
		{
			foreach (ImageCodecInfo ci in ImageCodecInfo.GetImageEncoders())
				_infos[ci.MimeType] = ci;

			var jpeg = FindCodecInfo("image/jpeg");
			foreach(var mime in new[] { "image/pjpeg" , "image/jpg/file" , "image/jpg" })
				if (!_infos.ContainsKey(mime))
					_infos.Add(mime, jpeg);
		}
		public static ImageCodecInfo FindCodecInfo(string mime)
		{
			ImageCodecInfo ici;
			if (_infos.TryGetValue(mime, out ici))
				return ici;
			throw new NotSupportedException("不支持这种图片类型：" + mime);
		}

		public static ImageCodecInfo FindCodecInfo(Guid guid)
		{
			foreach (KeyValuePair<string, ImageCodecInfo> p in _infos)
			{
				if (p.Value.FormatID == guid)
					return p.Value;
			}
			return null;
		}
	
	}


}
