using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.FrontEndContents.SiteConfigModels
{

	public class SiteModel
    {
		[StringLength(100)]
		[Display(Name="名称")]
		[Ignore]
		public string name { get; set; }

		[Display(Name = "页面列表")]
		[TreeNodes]
		public PageModel[] pages { get; set; }
    }
	public class PageModel
	{
		[Display(Name = "页面标示")]
		[StringLength(50)]
		[Required]
		[Key]
		public string ident { get; set; }

		[Display(Name = "页面名称")]
		[StringLength(50)]
		[Required]
		public string name { get; set; }

		[Display(Name = "页面备注")]
		[StringLength(100)]
		public string remarks { get; set; }

		[Display(Name = "是否禁用")]
		public bool? disabled { get; set; }

		[Display(Name = "包含页面内容")]
		public string[] includes { get; set; }

		[TreeNodes]
		[Display(Name = "页面块")]
		public BlockModel[] blocks { get; set; }
	}
	public class BlockModel
	{
		[Display(Name = "页面块标示")]
		[StringLength(50)]
		[Required]
		[Key]
		public string ident { get; set; }

		[Display(Name = "页面块名称")]
		[StringLength(50)]
		public string name { get; set; }

		[Display(Name = "页面块备注")]
		[StringLength(50)]
		public string remarks { get; set; }

		[Display(Name = "是否禁用")]
		public bool? disabled { get; set; }

		[TreeNodes]
		[Display(Name = "块内容")]
		public BlockContentModel[] contents { get; set; }
	}
	public class BlockContentModel
	{
		[Display(Name = "名称")]
		[StringLength(100)]
		[Layout(1)]
		public string name { get; set; }

		[Display(Name = "显示内容")]
		[EntityIdent(typeof(Content))]
		[Layout(2)]
		public long? content { get; set; }

		[Display(Name = "内容配置")]
		[StringLength(100)]
		[Layout(3)]
		public string contentConfig { get; set; }

		[Display(Name = "视图引擎")]
		[StringLength(100)]
		[Required]
		[Layout(4)]
		public string render { get; set; }

		[Display(Name = "视图")]
		[StringLength(100)]
		[Required]
		[Layout(5)]
		public string view { get; set; }

		[Display(Name = "视图配置")]
		[StringLength(100)]
		[Layout(6)]
		public string viewConfig { get; set; }

		[Display(Name = "内容块图片")]
		[Image]
		[Layout(7, 1)]
		public string image { get; set; }
		[Display(Name = "内容块图标")]
		[Image]
		[Layout(7, 2)]
		public string icon { get; set; }

		[Display(Name = "字体图标类")]
		[StringLength(50)]
		[Layout(7, 3)]
		public string fontIcon { get; set; }

		[Display(Name = "链接")]
		[StringLength(200)]
		public string uri { get; set; }

		[Display(Name = "主标题")]
		[StringLength(100)]
		public string title1 { get; set; }
		[Display(Name = "次标题1")]
		[StringLength(100)]
		[Layout(9, 1)]
		public string title2 { get; set; }

		[Display(Name = "次标题2")]
		[StringLength(100)]
		[Layout(9, 2)]
		public string title3 { get; set; }

		[Display(Name = "摘要")]
		[StringLength(200)]
		public string summary { get; set; }


		[Display(Name = "是否禁用")]
		public bool? disabled { get; set; }
	}
}
