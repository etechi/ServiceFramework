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

using SF.Auth;
using SF.Metadata;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Settings
{
	[Comment("HTTP设置")]
	public class HttpSetting
	{

		[Comment(Name = "主域名", Description = "主域名，格式如： www.abc.com")]
		public string Domain { get; set; }

		[Comment(Name = "启用HTTPS模式", Description = "启用后，访问普通页面的用户将引导至通过HTTPS协议访问")]
		public bool HttpsMode { get; set; }

		[Comment(Name = "HTTP基础路径", Description = "使用方式: <img src=\"@Html.ResBase()path1/path2/image.gif\"/>")]
		public string HttpRoot { get; set; }
		
		[Comment(Name = "资源文件基础路径", Description = "默认为HTTP基础路径")]
		public string ResBase { get; set; }

		[Comment(Name = "图片资源文件基础路径", Description = "默认为资源文件基础路径")]
		public string ImageResBase { get; set; }

	}
}
