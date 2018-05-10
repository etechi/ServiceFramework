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
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Security
{
	public class CaptchaImage
	{
		public string CodePrefix { get; set; }
		public string Image { get; set; }
	}
	public class CaptchaImageCreateArgument
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public string Target { get; set; }
		public string BgColor { get; set; }
		public string ForeColor { get; set; }
	}
	[NetworkService]
	public interface ICaptchaImageService
	{
		[AnonymousAllowed]
		Task<CaptchaImage> CreateImage(CaptchaImageCreateArgument Arg);

		Task<bool> Validate(string Target,string Code);
	}
}
