using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Entities;
using SF.Data;
using SF.Data.Models;

namespace SF.Management.FrontEndServices.DataModels
{

	[Table("FESite")]
    [Comment(GroupName= "界面管理服务", Name = "站点配置")]
    public class Site<TSite, TSiteTemplate> :
		ObjectEntityBase<string>
		where TSite : Site<TSite, TSiteTemplate>
		where TSiteTemplate : SiteTemplate<TSite, TSiteTemplate>
	{
	
		[Index]
        [Display(Name = "站点模板ID")]
        public int TemplateId{get;set;}

		[ForeignKey(nameof(TemplateId))]
		public TSiteTemplate Template { get; set; }
	}
}
