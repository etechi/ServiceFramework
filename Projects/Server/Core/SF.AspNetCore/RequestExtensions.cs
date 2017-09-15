using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.AspNetCore
{
	public static class RequestExtensions
	{
		public static Uri Uri(this HttpRequest request)
		{
			var builder = new UriBuilder();
			builder.Scheme = request.Scheme;
			builder.Host = request.Host.Host;
			if(request.Host.Port.HasValue)
				builder.Port = request.Host.Port.Value;
			builder.Path = request.Path;
			builder.Query = request.QueryString.ToUriComponent();
			return builder.Uri;
		}
	}
}
