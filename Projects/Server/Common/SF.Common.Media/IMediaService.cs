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

using SF.Sys.Auth;
using SF.Sys.NetworkService;
using System.Net.Http;
using System.Threading.Tasks;

namespace SF.Common.Media
{
	public class CopyResult
	{
		public string Id { get; set; }
	}
	/// <summary>
	/// 媒体附件支持
	/// </summary>
	[NetworkService]
    public interface IMediaService
    {
		[DefaultAuthorizeAttribute]
		[HeavyMethod]
		Task<HttpResponseMessage> Upload(bool returnJson = false);

		[DefaultAuthorizeAttribute]
		Task<string> Clip(string src, double x, double y, double w, double h);

		Task<HttpResponseMessage> Get(string id, string format = null);


		[DefaultAuthorizeAttribute]
		[HeavyMethod]
		Task<CopyResult> CopyImage(string uri);

	}
}
