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
using SF.Common.Members.Models;
using SF.Sys.Annotations;
using System.ComponentModel.DataAnnotations;

namespace SFShop
{
	public class SFShopSetting
    {
		///<title>PC站点默认帮助文档</title>
		/// <summary>
		/// 默认帮助中心文档
		/// </summary>
		/// <group>帮助中心</group>
		[Required]
		public long PCHelpCenterDefaultDocId { get; set; }

		///<title>主产品目录</title>
		/// <group>产品</group>
		[Required]
		[EntityIdent(typeof(ProductInternal))]
		public long MainProductCategoryId { get; set; }

		///<title>默认卖家</title>
		/// <group>产品</group>
		[Required]
		[EntityIdent(typeof(MemberInternal))]
		public long DefaultSellerId { get; set; }


	}


}
