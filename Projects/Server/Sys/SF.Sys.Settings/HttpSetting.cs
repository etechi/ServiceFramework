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


namespace SF.Sys.Settings
{
	/// <summary>
	/// HTTP设置
	/// </summary>
	public class HttpSetting
	{

		///<title>主域名</title>
		/// <summary>
		/// 主域名，格式如： www.abc.com
		/// </summary>
		public string Domain { get; set; }

		///<title>启用HTTPS模式</title>
		/// <summary>
		/// 启用后，访问普通页面的用户将引导至通过HTTPS协议访问
		/// </summary>
		public bool HttpsMode { get; set; }

		///<title>HTTP基础路径</title>
		/// <summary>
		/// 使用方式: <img src="@Html.ResBase()path1/path2/image.gif"/>
		/// </summary>
		public string HttpRoot { get; set; }

		///<title>Api基础路径</title>
		/// <summary>
		/// 默认为HTTP基础路径
		/// </summary>
		public string ApiBase { get; set; }

		///<title>H5App基础路径</title>
		/// <summary>
		/// 默认为HTTP基础路径
		/// </summary>
		public string H5AppBase { get; set; }


		///<title>资源文件基础路径</title>
		/// <summary>
		/// 默认为HTTP基础路径
		/// </summary>
		public string ResBase { get; set; }

		///<title>图片资源文件基础路径</title>
		/// <summary>
		/// 默认为资源文件基础路径
		/// </summary>
		public string ImageBase { get; set; }
	}

	public static class HttpSettingExtension
	{
		public static string GetUrlBase(this HttpSetting setting)
			=> setting.HttpRoot ?? $"{(setting.HttpsMode ? "https" : "http")}://{setting.Domain}";

		public static string GetH5AppUrlBase(this HttpSetting setting)
			=> setting.H5AppBase ?? setting.GetUrlBase() + "/h5/";

		public static string GetResUrlBase(this HttpSetting setting)
			=> setting.ResBase ?? setting.GetUrlBase() + "/r/";

		public static string GetImageUrlBase(this HttpSetting setting)
			=> setting.ImageBase ?? setting.GetUrlBase() + "/r/";

		public static string GetApiUrlBase(this HttpSetting setting)
			=> setting.ApiBase ?? setting.GetUrlBase() + "/api/";

	}

}
