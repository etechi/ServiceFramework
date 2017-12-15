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

using System.Collections.Generic;
using System.Linq;
namespace SF.Sys.Auth
{
	public static class PredefinedClaimTypes {
		/// <summary>
		/// 本地账号
		/// </summary>
		public  const string LocalAccount = "acc";
		/// <summary>
		/// 本地ID
		/// </summary>
		public  const string Subject = "sub";
		/// <summary>
		/// 电话
		/// </summary>
		public const string Phone = "phone";
		/// <summary>
		/// 地址
		/// </summary>
		public const string Address = "address";
		/// <summary>
		/// 电子邮件
		/// </summary>
		public const string EMail = "email";
		/// <summary>
		/// 姓名
		/// </summary>
		public const string Name = "name";

		/// <summary>
		/// 图标
		/// </summary>
		public const string Icon = "icon";

		/// <summary>
		/// 头像
		/// </summary>
		public const string Image = "image";

		/// <summary>
		/// 国家
		/// </summary>
		public const string Country = "country";

		/// <summary>
		/// 省份
		/// </summary>
		public const string Province = "province";

		/// <summary>
		/// 城市
		/// </summary>
		public const string City= "city";

		/// <summary>
		/// 性别
		/// </summary>
		public const string Sex = "sex";

		/// <summary>
		/// 微信开放平台ID
		/// </summary>
		public const string WeiXinOpenPlatformId = "wx.open";
		/// <summary>
		/// 微信公众号ID
		/// </summary>
		public const string WeiXinMPId = "wx.mp";

		/// <summary>
		/// 微信统一ID
		/// </summary>
		public const string WeiXinUnionId = "wx.uid";

		/// <summary>
		/// 测试ID
		/// </summary>
		public const string TestId = "test.id";

		public static Dictionary<string, string> Items { get; } =
			typeof(PredefinedClaimTypes).
			GetFields(
				System.Reflection.BindingFlags.Static | 
				System.Reflection.BindingFlags.Public | 
				System.Reflection.BindingFlags.GetField
				)
			.ToDictionary(f => f.Name, f => (string)f.GetValue(null))
			;
	}
}
