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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Entities;

namespace SF.Management.FrontEndContents.DataModels
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
