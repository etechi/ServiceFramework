using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndServices
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
		int Id { get; }
		string Name { get; }
		string Category { get; }
		string ProviderType { get; }
		string ProviderConfig { get; }
		bool Disabled { get; }

	}
	public class ContentItem : IContentItem
	{
		[Layout(10,1)]
		[Display(Name = "大图")]
		[Image]
		public string Image { get; set; }

		[Display(Name = "图标")]
		[Layout(10, 2)]
		[Image]
		public string Icon { get; set; }

		[Display(Name = "字体图标类")]
		[StringLength(50)]
		[Layout(10, 3)]
		public string FontIcon { get; set; }

		[Display(Name = "主标题")]
		[TableVisible(10)]
		[StringLength(100)]
		public string Title1 { get; set; }

		[Display(Name = "次标题1")]
		[StringLength(100)]
		[Layout(20, 1)]
		public string Title2 { get; set; }

		[Display(Name = "次标题2")]
		[StringLength(100)]
		[Layout(20,2)]
		public string Title3 { get; set; }

		[Display(Name = "摘要")]
		[StringLength(200)]
		public string Summary { get; set; }

		[TableVisible]
		[Display(Name = "链接")]
		[StringLength(200)]
		public string Uri { get; set; }

		[Display(Name = "链接目标")]
		[StringLength(50)]
		public string UriTarget { get; set; }

		[Display(Name = "子项目")]
		[TreeNodes]
		public ContentItem[] Items { get; set; }
	}
    [EntityObject("界面内容")]
    public class Content:ContentItem, IContent,IEntityWithId<int>
	{
		[Key]
		[Display(Name = "ID", Prompt = "保存后自动产生")]
		[ReadOnly(true)]
		[Layout(1)]
		[TableVisible(1)]
		public int Id { get; set; }

		[Display(Name = "内容分类")]
		[StringLength(50)]
		[TableVisible]
		[Required]
		public string Category { get; set; }

		[StringLength(50)]
		[Display(Name = "内容名称")]
		[TableVisible]
		[Required]
		public string Name { get; set; }


		[Display(Name = "提供者类型")]
		[StringLength(50)]
		[TableVisible]
		[Layout(3)]
		public string ProviderType { get; set; }

		[Display(Name = "提供者配置")]
		[StringLength(100)]
		[Layout(4)]
		public string ProviderConfig { get; set; }

		[Display(Name = "是否禁用")]
		[TableVisible]
		public bool Disabled { get; set; }

    }
}
