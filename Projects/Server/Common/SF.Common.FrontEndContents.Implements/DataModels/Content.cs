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


using SF.Sys.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Common.FrontEndContents.DataModels
{
	/// <summary>
	/// 界面内容
	/// </summary>
	[Table("FrontContent")]
    public class Content :
		IEntityWithId<long>
	{
		/// <summary>
		/// ID
		/// </summary>
		[Key]
        public long Id { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[Required]
		[MaxLength(200)]
        public string Name { get; set; }

		/// <summary>
		/// 分类
		/// </summary>
		[Required]
		[MaxLength(200)]
        public string Category { get; set; }


		/// <summary>
		/// 字体图标
		/// </summary>
		[MaxLength(100)]
        public string FontIcon{get;set;}

		/// <summary>
		/// 图片图标
		/// </summary>
		[MaxLength(200)]
        public string Icon { get; set; }

		/// <summary>
		/// 大图片
		/// </summary>
		[MaxLength(200)]
        public string Image { get; set; }


		/// <summary>
		/// 摘要
		/// </summary>
		public string Summary { get; set; }

		/// <summary>
		/// 标题1
		/// </summary>
		[MaxLength(200)]
        public string Title1 { get; set; }

		/// <summary>
		/// 标题2
		/// </summary>
		[MaxLength(200)]
        public string Title2 { get; set; }

		/// <summary>
		/// 标题3
		/// </summary>
		[MaxLength(200)]
        public string Title3 { get; set; }

		/// <summary>
		/// 链接地址
		/// </summary>
		[MaxLength(200)]
        public string Uri { get; set; }

		/// <summary>
		/// 链接打开目标
		/// </summary>
		[MaxLength(100)]
        public string UriTarget { get; set; }

		/// <summary>
		/// 数据提供者类型
		/// </summary>
		[MaxLength(100)]
        public string ProviderType { get; set; }

		/// <summary>
		/// 数据提供者配置
		/// </summary>
        public string ProviderConfig { get; set; }

		/// <summary>
		/// 是否有效
		/// </summary>
		public bool Disabled { get; set; }

		/// <summary>
		/// 子项配置数据
		/// </summary>
		public string ItemsData { get; set; }
	}
}
