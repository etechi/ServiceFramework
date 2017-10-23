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

using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents
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
		[Layout(10,1)]
		[Comment(Name = "大图")]
		[Image]
		public string Image { get; set; }

		[Comment(Name = "图标")]
		[Layout(10, 2)]
		[Image]
		public string Icon { get; set; }

		[Comment(Name = "字体图标类")]
		[StringLength(50)]
		[Layout(10, 3)]
		public string FontIcon { get; set; }

		[Comment(Name = "主标题")]
		[TableVisible(10)]
		[StringLength(100)]
		public string Title1 { get; set; }

		[Comment(Name = "次标题1")]
		[StringLength(100)]
		[Layout(20, 1)]
		public string Title2 { get; set; }

		[Comment(Name = "次标题2")]
		[StringLength(100)]
		[Layout(20,2)]
		public string Title3 { get; set; }

		[Comment(Name = "摘要")]
		[StringLength(200)]
		public string Summary { get; set; }

		[TableVisible]
		[Comment(Name = "链接")]
		[StringLength(200)]
		public string Uri { get; set; }

		[Comment(Name = "链接目标")]
		[StringLength(50)]
		public string UriTarget { get; set; }

		[Comment(Name = "子项目")]
		[TreeNodes]
		public ContentItem[] Items { get; set; }
	}
    [EntityObject]
    public class Content:ContentItem, IContent,IEntityWithId<long>
	{
		[Key]
		[Comment(Name = "ID", Prompt = "保存后自动产生")]
		[ReadOnly(true)]
		[Layout(1)]
		[TableVisible(1)]
		public long Id { get; set; }

		[Comment(Name = "内容分类")]
		[StringLength(50)]
		[TableVisible]
		[Required]
		public string Category { get; set; }

		[StringLength(50)]
		[Comment(Name = "内容名称")]
		[TableVisible]
		[Required]
		public string Name { get; set; }


		[Comment(Name = "提供者类型")]
		[StringLength(50)]
		[TableVisible]
		[Layout(3)]
		public string ProviderType { get; set; }

		[Comment(Name = "提供者配置")]
		[StringLength(100)]
		[Layout(4)]
		public string ProviderConfig { get; set; }

		[Comment(Name = "是否禁用")]
		[TableVisible]
		public bool Disabled { get; set; }

    }
}
