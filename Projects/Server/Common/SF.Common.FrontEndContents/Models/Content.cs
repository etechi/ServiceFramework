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

using SF.Sys.Annotations;
using SF.Sys.Entities;
using SF.Sys.Entities.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.FrontEndContents
{
	public interface IContentItem
	{
		string Image { get; }
		string Icon { get; }
		string FontIcon { get; }
		string Uri { get; }
		string UriTarget { get; }
		string Title1 { get; }
		string Title2 { get; }
		string Title3 { get; }
		string Summary { get; }
		ContentItem[] Items { get; }
	}
	public interface IContent : IContentItem
	{
		long Id { get; }
		string Name { get; }
		string Category { get; }
		string ProviderType { get; }
		string ProviderConfig { get; }
		bool Disabled { get; }

	}
	public class ContentItem : IContentItem
	{
		/// <summary>
		/// 大图
		/// </summary>
		[Layout(10,1)]
		[Image]
		public string Image { get; set; }

		/// <summary>
		/// 图标
		/// </summary>
		[Layout(10, 2)]
		[Image]
		public string Icon { get; set; }

		/// <summary>
		/// 字体图标类
		/// </summary>
		[StringLength(50)]
		[Layout(10, 3)]
		public string FontIcon { get; set; }

		/// <summary>
		/// 主标题
		/// </summary>
		[TableVisible(10)]
		[StringLength(100)]
		public string Title1 { get; set; }

		/// <summary>
		/// 次标题1
		/// </summary>
		[StringLength(100)]
		[Layout(20, 1)]
		public string Title2 { get; set; }

		/// <summary>
		/// 次标题2
		/// </summary>
		[StringLength(100)]
		[Layout(20,2)]
		public string Title3 { get; set; }

		/// <summary>
		/// 摘要
		/// </summary>
		[StringLength(200)]
		public string Summary { get; set; }

		/// <summary>
		/// 链接
		/// </summary>
		[TableVisible]
		[StringLength(200)]
		public string Uri { get; set; }

		/// <summary>
		/// 链接目标
		/// </summary>
		[StringLength(50)]
		public string UriTarget { get; set; }

		/// <summary>
		/// 子项目
		/// </summary>
		[TreeNodes]
		public ContentItem[] Items { get; set; }
	}
    [EntityObject]
    public class Content:ContentItem, IContent,IEntityWithId<long>
	{
		/// <summary>
		/// ID
		/// </summary>
		/// <prompt>保存后自动产生</prompt>
		[Key]
		[ReadOnly(true)]
		[Layout(1)]
		[TableVisible(1)]
		public long Id { get; set; }

		/// <summary>
		/// 内容分类
		/// </summary>
		[StringLength(50)]
		[TableVisible]
		[Required]
		public string Category { get; set; }


		/// <summary>
		/// 内容名称
		/// </summary>
		[StringLength(50)]
		[TableVisible]
		[Required]
		public string Name { get; set; }


		/// <summary>
		/// 提供者类型
		/// </summary>
		[StringLength(50)]
		[TableVisible]
		[Layout(3)]
		public string ProviderType { get; set; }

		/// <summary>
		/// 提供者配置
		/// </summary>
		[StringLength(100)]
		[Layout(4)]
		public string ProviderConfig { get; set; }

		/// <summary>
		/// 是否禁用
		/// </summary>
		[TableVisible]
		public bool Disabled { get; set; }

    }
}
