using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using SF.Core.Serialization;
using SF.Auth;
using SF.Metadata;
using System.IO;
using System.Web;

namespace SF.Core.NetworkService
{
	public class UploadedFileCollection : IUploadedFileCollection
	{
		class UploadedFile : IUploadedFile
		{
			public string Key { get; set; }
			public long ContentLength { get; set; }

			public string ContentType { get; set; }

			public string FileName { get; set; }
			public Stream InputStream { get; set; }
		}
		public IUploadedFile[] Files
		{
			get
			{
				if (HttpContext.Current == null)
					return Array.Empty<IUploadedFile>();
				var files = HttpContext.Current.Request.Files;
				if(files.Count==0)
					return Array.Empty<IUploadedFile>();

				var re = new IUploadedFile[files.Count];
				return files.AllKeys
					.Select(k => new { key = k, file = files[k] })
					.Select(file => new UploadedFile
					{
						Key = file.key,
						ContentLength = file.file.ContentLength,
						ContentType = file.file.ContentType,
						FileName = file.file.FileName,
						InputStream = file.file.InputStream
					}).ToArray();
			}
		}
	}



}
