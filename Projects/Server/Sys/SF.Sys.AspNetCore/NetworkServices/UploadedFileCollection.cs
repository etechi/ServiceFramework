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

using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using SF.Sys.NetworkService;
using System;
using SF.Sys.Services;

namespace SF.Sys.AspNetCore.NetworkServices
{
	public class UploadedFileCollection : IUploadedFileCollection
	{
		class UploadedFile : IUploadedFile
		{
			public IFormFile file { get; set; }
			public string Key => file.Name;
			public long ContentLength => file.Length;

			public string ContentType => file.ContentType;

			public string FileName => file.FileName;

			public Stream OpenStream() => file.OpenReadStream();
		}
		public Microsoft.AspNetCore.Http.IHttpContextAccessor HttpContextAccessor { get; }
		public UploadedFileCollection(IServiceProvider Services)
		{
			HttpContextAccessor = Services.Resolve<IHttpContextAccessor>();
		}
		public IUploadedFile[] Files
		{
			get
			{
				var req = HttpContextAccessor.HttpContext.Request;

				var re = new IUploadedFile[req.Form.Files.Count];
				return req.Form.Files
					.Select(file => new UploadedFile
					{
						file=file
					}).ToArray();
			}
		}
	}



}
