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

using System.Text;
using Microsoft.AspNetCore.Mvc;
using SF.Sys.NetworkService;

namespace SF.Sys.AspNetCore.NetworkServices
{
	public class ResultFactory : IResultFactory
	{
		public object Content(string Text, string Mime, Encoding Encoding, string FileName)
		{
			if (FileName != null)
				return new FileContentResult(
					(Encoding ?? Encoding.UTF8).GetBytes(Text),
					Mime
					)
				{
					FileDownloadName = FileName
				};
			return new ContentResult { Content = Text, ContentType = Mime };
		}

		public object Content(byte[] Data, string Mime, Encoding Encoding, string FileName)
		{
			return new FileContentResult(
					Data,
					Mime
					)
			{
				FileDownloadName = FileName
			};
		}

		public object File(string Path, string Mime, string FileName = null)
		{
			return new PhysicalFileResult(
				Path,
				Mime
				)
			{
				FileDownloadName = FileName
			};
		}

		public object Json<T>(T obj)
		{
			return new JsonResult(obj);
		}

		public object LocalRedirect(string Path)
		{
			return new LocalRedirectResult(Path);
		}

		public object Redirect(string Path)
		{
			return new RedirectResult(Path);
		}
	}



}
