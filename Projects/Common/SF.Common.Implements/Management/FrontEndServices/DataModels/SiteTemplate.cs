using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Entities;
using SF.Data.Models;

namespace SF.Management.FrontEndServices.DataModels
{

	[Table("FESiteTemplate")]
    [Comment(GroupName ="界面管理服务",Name ="站点模板",Description ="记录站点模板配置数据")]
	public class SiteTemplate<TSite,TSiteTemplate> :
		ObjectEntityBase<long>
		where TSite: Site<TSite,TSiteTemplate>
		where TSiteTemplate : SiteTemplate<TSite,TSiteTemplate>
	{

        [Display(Name = "模板配置数据")]
        public string Data { get; set; }
	}
}
