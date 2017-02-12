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
	public class HttpRequestSource : IHttpRequestSource
	{
		public HttpRequestMessage Request
		{
			get;set;
		}
	}

}
