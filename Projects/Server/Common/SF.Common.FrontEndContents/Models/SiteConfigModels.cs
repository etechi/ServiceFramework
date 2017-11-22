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

using SF.Sys.Entities.Annotations;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.FrontEndContents.SiteConfigModels
{

	public class SiteModel
    {
		/// <summary>
		/// 名称
		/// </summary>
		[StringLength(100)]
		[Ignore]
		public string name { get; set; }

		/// <summary>
		/// 页面列表
		/// </summary>
		[TreeNodes]
		public PageModel[] pages { get; set; }
    }
	public class PageModel
	{
		/// <summary>
		/// 页面标示
		/// </summary>
		[StringLength(50)]
		[Required]
		[Key]
		public string ident { get; set; }

		/// <summary>
		/// 页面名称
		/// </summary>
		[StringLength(50)]
		[Required]
		public string name { get; set; }

		/// <summary>
		/// 页面备注
		/// </summary>
		[StringLength(100)]
		public string remarks { get; set; }

		/// <summary>
		/// 是否禁用
		/// </summary>
		public bool? disabled { get; set; }
		/// <summary>
		/// 包含页面内容
		/// </summary>
		public string[] includes { get; set; }

		/// <summary>
		/// 页面块
		/// </summary>
		[TreeNodes]
		public BlockModel[] blocks { get; set; }
	}
	public class BlockModel
	{
		/// <summary>
		/// 页面块标示
		/// </summary>
		[StringLength(50)]
		[Required]
		[Key]
		public string ident { get; set; }

		/// <summary>
		/// 页面块名称
		/// </summary>
		[StringLength(50)]
		public string name { get; set; }

		/// <summary>
		/// 页面块备注
		/// </summary>
		[StringLength(50)]
		public string remarks { get; set; }

		/// <summary>
		/// 是否禁用
		/// </summary>
		public bool? disabled { get; set; }

		/// <summary>
		/// 块内容
		/// </summary>
		[TreeNodes]
		public BlockContentModel[] contents { get; set; }
	}
	public class BlockContentModel
	{
		/// <summary>
		/// 名称
		/// </summary>
		[StringLength(100)]
		[Layout(1)]
		public string name { get; set; }

		/// <summary>
		/// 显示内容
		/// </summary>
		[EntityIdent(typeof(Content))]
		[Layout(2)]
		public long? content { get; set; }

		/// <summary>
		/// 内容配置
		/// </summary>
		[StringLength(100)]
		[Layout(3)]
		public string contentConfig { get; set; }

		/// <summary>
		/// 视图引擎
		/// </summary>
		[StringLength(100)]
		[Required]
		[Layout(4)]
		public string render { get; set; }

		/// <summary>
		/// 视图
		/// </summary>
		[StringLength(100)]
		[Required]
		[Layout(5)]
		public string view { get; set; }

		/// <summary>
		/// 视图配置
		/// </summary>
		[StringLength(100)]
		[Layout(6)]
		public string viewConfig { get; set; }

		/// <summary>
		/// 内容块图片
		/// </summary>
		[Image]
		[Layout(7, 1)]
		public string image { get; set; }
		/// <summary>
		/// 内容块图标
		/// </summary>
		[Image]
		[Layout(7, 2)]
		public string icon { get; set; }

		/// <summary>
		/// 字体图标类
		/// </summary>
		[StringLength(50)]
		[Layout(7, 3)]
		public string fontIcon { get; set; }

		/// <summary>
		/// 链接
		/// </summary>
		[StringLength(200)]
		public string uri { get; set; }

		/// <summary>
		/// 主标题
		/// </summary>
		[StringLength(100)]
		public string title1 { get; set; }
		/// <summary>
		/// 次标题1
		/// </summary>
		[StringLength(100)]
		[Layout(9, 1)]
		public string title2 { get; set; }

		/// <summary>
		/// 次标题2
		/// </summary>
		[StringLength(100)]
		[Layout(9, 2)]
		public string title3 { get; set; }

		/// <summary>
		/// 摘要
		/// </summary>
		[StringLength(200)]
		public string summary { get; set; }


		/// <summary>
		/// 是否禁用
		/// </summary>
		public bool? disabled { get; set; }
	}
}
