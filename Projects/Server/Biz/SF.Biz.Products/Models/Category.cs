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

using SF.Biz.Shops.Managements;
using SF.Sys.Annotations;
using SF.Sys.Entities;
using SF.Sys.Entities.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Products
{
	[EntityObject]
    public class CategoryInternal:UIObjectEntityBase
	{
		

		///<title>名称</title>
		/// <summary>
		/// 内部管理使用，比如：2016夏季促销商品
		/// </summary>
		[TableVisible]
		[Required]
		[StringLength(100)]
        [Layout(2)]
        public override string Name { get; set; }


		/// <summary>
		/// 标题
		/// </summary>
		[Required]
		[TableVisible]
		[StringLength(100)]
        [Layout(3)]
        public override string Title { get; set; }


		///<title>销售人员</title>
		/// <summary>
		/// 一般默认即可
		/// </summary>
        [Required]
        [Layout(4)]
        public long SellerId { get; set; }

		/// <summary>
		/// 父目录
		/// </summary>
		[EntityIdent(typeof(CategoryInternal), nameof(ParentName), IsTreeParentId = true, ScopeField = nameof(SellerId))]
        [Layout(5)]
        public long? ParentId { get; set; }

		/// <summary>
		/// 父目录
		/// </summary>
		[Ignore]
		[TableVisible]
        [Layout(6)]
        public string ParentName { get; set; }


		///<title>标签</title>
		/// <summary>
		/// 用于控制前端显示的标签，一般留空即可
		/// </summary>
		[TableVisible]
		[StringLength(50)]
        [Layout(7)]
        public string Tag { get; set; }

		///<title>描述</title>
		/// <summary>
		/// 商品分类的描述，供前端显示使用
		/// </summary>
		[StringLength(100)]
        [Layout(8)]
        public override string Description { get; set; }

		///<title>图片</title>
		/// <summary>
		/// 商品分类的大图片，供前端显示使用
		/// </summary>
		[Image]
        [Layout(9)]
        public override string Image { get; set; }

		///<title>图标</title>
		/// <summary>
		/// 商品分类的小图片，供前端显示使用
		/// </summary>
		[Image]
        [Layout(10)]
        public override string Icon { get; set; }

		///<title>广告图</title>
		/// <summary>
		/// PC栏目页面广告图
		/// </summary>
        [Layout(11)]
        [Image]
        [MaxLength(200)]
        public string BannerImage { get; set; }

		///<title>广告图链接</title>
		/// <summary>
		/// PC栏目页面广告图链接
		/// </summary>
        [Layout(12)]
        [MaxLength(200)]
        public string BannerUrl { get; set; }

		///<title>移动站广告图</title>
		/// <summary>
		/// 移动站栏目页面广告图
		/// </summary>
        [Layout(13)]
        [Image]
        [MaxLength(200)]
        public string MobileBannerImage { get; set; }

		///<title>移动站广告图链接</title>
		/// <summary>
		/// 移动栏目页面广告图链接
		/// </summary>
        [Layout(14)]
        [MaxLength(200)]
        public string MobileBannerUrl { get; set; }

        /// <summary>
        /// 店铺
        /// </summary>
        [EntityIdent(typeof(ShopInternal),nameof(ShopName))]
        public long? ShopId { get; set; }

        /// <summary>
        /// 店铺
        /// </summary>
        [Ignore]
        [TableVisible]
        public string ShopName { get; set; }

        ///<title>列表排位</title>
        /// <summary>
        /// 商品分类在分类列表中的排位，小的在前
        /// </summary>
        [TableVisible]
        [Optional]
        public int Order { get; set; }

		/// <summary>
		/// 对象状态
		/// </summary>
		[TableVisible]
		[Required]
		public EntityLogicState ObjectState { get; set; }

		[Ignore]
		public CategoryInternal[] Children { get; set; }

		[Ignore]
		public long[] Items { get; set; }
    }

}
