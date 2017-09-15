using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Data;
using SF.Entities;

namespace SF.Management.FrontEndContents.DataModels
{

	[Table("FrontSite")]
    [Comment(GroupName= "界面管理服务", Name = "站点配置")]

    public class Site<TSite, TSiteTemplate> :
		IEntityWithId<string>
		where TSite : Site<TSite, TSiteTemplate>
		where TSiteTemplate : SiteTemplate<TSite, TSiteTemplate>
	{
		[Key]
		[MaxLength(100)]
        [Display(Name="Id")]
		public string Id{get;set;}

		[MaxLength(100)]
        [Display(Name = "站点名称")]
        public string Name { get; set; }
		[Index]
        [Display(Name = "站点模板ID")]
        public long TemplateId{get;set;}

		[ForeignKey(nameof(TemplateId))]
		public TSiteTemplate Template { get; set; }
	}

	public class Site : Site<Site, SiteTemplate>
	{ }
}
