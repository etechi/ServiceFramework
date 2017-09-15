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
	public static class HeaderDictionaryExtensions
	{
		public static string GetValue(this IHeaderDictionary headers, string Name)
			=> headers[Name].ToString();
		public static string UserAgent(this IHeaderDictionary headers)
			=> headers.GetValue("User-Agent");
	}
}
