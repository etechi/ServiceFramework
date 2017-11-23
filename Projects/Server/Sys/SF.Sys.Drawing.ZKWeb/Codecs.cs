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
using System.Collections.Generic;
using System.DrawingCore.Imaging;

namespace SF.Sys.Drawing.dotNetFramework
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
