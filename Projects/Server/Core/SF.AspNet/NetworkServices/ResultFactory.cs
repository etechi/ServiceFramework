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
using System.Text;
using System.Web.Http.Results;

namespace SF.AspNet.NetworkService
{
	class ResultFactory : IResultFactory
	{
		public Lazy<ControllerSource> ControllerSource { get; }
		public ResultFactory(Lazy<ControllerSource> ControllerSource)
		{
			this.ControllerSource = ControllerSource;
		}
		public object Content(string Text, string Mime, Encoding Encoding, string FileName)
		{
			var re = HttpResponse.Text(Text, Mime, Encoding);

			return re;
		}

		public object Content(byte[] Data, string Mime, Encoding Encoding, string FileName)
		{
			var re = HttpResponse.ByteArray(Data, Mime);

			return re;
		}

		public object File(string Path, string Mime, string FileName = null)
		{
			var re = HttpResponse.File(Path, Mime);
			return re;
		}

		public object Json<T>(T obj)
		{
			var re = obj;
			return re;
		}

		public object LocalRedirect(string Path)
		{
			return new RedirectResult(new Uri(Path), ControllerSource.Value.Controller);
		}

		public object Redirect(string Path)
		{
			return new RedirectResult(new Uri(Path), ControllerSource.Value.Controller);
		}
	}



}
