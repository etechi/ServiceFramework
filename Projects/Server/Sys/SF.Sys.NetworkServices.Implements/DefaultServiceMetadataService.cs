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


using SF.Sys.IO;
using SF.Sys.NetworkService.Metadata;
using System.Reflection;

namespace SF.Sys.NetworkService
{
	public class DefaultServiceMetadataService : IServiceMetadataService
	{
		Library Library { get; }
		public DefaultServiceMetadataService(Library lib)
		{
			this.Library = lib;
		}
		public Library Json()
		{
			return Library;
		}

		public IContent Typescript(bool all = true)
		{
			var tb = new TypeScriptProxyBuilder(
				(c, a) =>
				all ||
				a.GrantInfo==null
				);
			var code = tb.Build(Library);
			return new StringContent
			{
				Content = code,
				ContentType = "text/javascript",
			};
		}
		public IContent TSD(string ApiName, bool all = true)
		{
			var tb = new TSDBuilder(
				ApiName,
				(c, a) =>
				all ||
				a.GrantInfo == null
				);
			var code = tb.Build(Library);
			return new StringContent
			{
				Content = code,
				ContentType = "text/javascript",
			};
		}
		public IContent Javascript(string ApiName,bool all = true)
		{
			var tb = new JavascriptProxyBuilder(
				ApiName,
				(c, a) =>
				all ||
				a.GrantInfo == null
				);
			var code = tb.Build(Library);
			return new StringContent
			{
				Content = code,
				ContentType = "text/javascript",
			};
		}
		public IContent Java(
			string CommonImports,
			string PackagePath, 
			bool all = true
			)
		{
			var tb = new JavaProxyBuilder(
				CommonImports,
				PackagePath,
				(c, a) =>
				all ||
				a.GrantInfo == null
				);
			return tb.Build(Library);
		}
		static Assembly CurAssembly { get; } = typeof(DefaultServiceMetadataService).Assembly;
		static string GetResPath(string Path)
		{
			return CurAssembly.GetName().Name + ".Html." + Path;
		}
		static string ReadResString(string Path)
		{
			return CurAssembly.GetManifestResourceStream(GetResPath(Path)).ReadString(null, true, true);
		}
		public IContent Angular()
		{
			return new StringContent
			{
				Content = ReadResString("Scripts.angular.min.js"),
				ContentType = "text/javascript",
			};
		}
		public IContent Script()
		{
			return new StringContent
			{
				Content = ReadResString("ApiMetadataView.js"),
				ContentType = "text/javascript",
			};
		}
		public IContent Html()
		{
			return new StringContent
			{
				Content = ReadResString("ApiMetadataView.html"),
				ContentType = "text/html",
			};
		}

	}

}
