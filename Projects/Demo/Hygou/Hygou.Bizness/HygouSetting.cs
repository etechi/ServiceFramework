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

using SF.Biz.Products;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hygou
{
    public class HygouSetting
    {
		[Required]
		[Comment(GroupName = "帮助中心", Name = "PC站点默认帮助文档", Description = "默认帮助中心文档")]
		public long PCHelpCenterDefaultDocId { get; set; }


		[Required]
		[Comment(GroupName = "产品", Name = "主产品目录", Description = "主产品目录")]
		[EntityIdent(typeof(ProductInternal))]
		public long MainProductCategoryId { get; set; }

		[Required]
		[Comment(GroupName = "产品", Name = "默认卖家")]
		[EntityIdent(typeof(SF.Users.Members.Models.MemberInternal))]
		public long DefaultSellerId { get; set; }


	}


}
