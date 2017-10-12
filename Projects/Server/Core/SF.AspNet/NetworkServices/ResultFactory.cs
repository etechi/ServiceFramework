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
