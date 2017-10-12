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
	[Table("FrontContent")]
    [Comment(GroupName = "界面管理服务", Name = "界面内容")]
    public class Content :
		IEntityWithId<long>
	{
		[Key]
        [Display(Name = "ID")]
        public long Id { get; set; }
		[Required]
		[MaxLength(200)]
        [Display(Name = "名称")]
        public string Name { get; set; }

		[Required]
		[MaxLength(200)]
        [Display(Name = "分类")]
        public string Category { get; set; }


		[MaxLength(100)]
        [Display(Name = "字体图标")]
        public string FontIcon{get;set;}

		[MaxLength(200)]
        [Display(Name = "图片图标")]
        public string Icon { get; set; }

		[MaxLength(200)]
        [Display(Name = "大图片")]
        public string Image { get; set; }


        [Display(Name = "摘要")]
        public string Summary { get; set; }

		[MaxLength(200)]
        [Display(Name = "标题1")]
        public string Title1 { get; set; }

		[MaxLength(200)]
        [Display(Name = "标题2")]
        public string Title2 { get; set; }

		[MaxLength(200)]
        [Display(Name = "标题3")]
        public string Title3 { get; set; }

		[MaxLength(200)]
        [Display(Name = "链接地址")]
        public string Uri { get; set; }

		[MaxLength(100)]
        [Display(Name = "链接打开目标")]
        public string UriTarget { get; set; }

		[MaxLength(100)]
        [Display(Name = "数据提供者类型")]
        public string ProviderType { get; set; }

        [Display(Name = "数据提供者配置")]
        public string ProviderConfig { get; set; }

        [Display(Name = "是否有效")]
        public bool Disabled { get; set; }

        [Display(Name = "子项配置数据")]
        public string ItemsData { get; set; }
	}
}
