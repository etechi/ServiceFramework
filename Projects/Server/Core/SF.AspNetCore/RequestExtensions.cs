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
