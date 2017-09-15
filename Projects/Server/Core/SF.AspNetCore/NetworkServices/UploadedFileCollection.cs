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
using SF.Core.NetworkService;
using Microsoft.AspNetCore.Http;

namespace SF.AspNetCore.NetworkServices
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
		public UploadedFileCollection(Microsoft.AspNetCore.Http.IHttpContextAccessor HttpContextAccessor)
		{
			this.HttpContextAccessor = HttpContextAccessor;
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
