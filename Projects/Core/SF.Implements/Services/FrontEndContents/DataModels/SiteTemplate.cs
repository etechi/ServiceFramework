using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Entities;

namespace SF.Services.FrontEndContents.DataModels
{

	[Table("FrontSiteTemplate")]
    [Comment(GroupName ="界面管理服务",Name ="站点模板",Description ="记录站点模板配置数据")]
	public class SiteTemplate<TSite,TSiteTemplate> :
		IEntityWithId<long>
		where TSite: Site<TSite,TSiteTemplate>
		where TSiteTemplate : SiteTemplate<TSite,TSiteTemplate>
	{
		[Key]
        [Display(Name ="ID")]
		public long Id { get; set; }

		[Required]
		[MaxLength(100)]
        [Display(Name = "模板名称")]
        public string Name{get;set;}

        [Display(Name = "模板配置数据")]
        public string Data { get; set; }
	}

	public class SiteTemplate : SiteTemplate<Site, SiteTemplate>
	{ }

}
