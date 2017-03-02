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
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace SF.AspNetCore.NetworkServices
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
