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

using SF.Sys.Annotations;
using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Products
{
	public interface IProductImage
	{
		string Image { get; }
		string Title { get; }
	}
	public interface IProductDescItem
	{
		string Image { get; }
		string Title { get; }
	}


	public class ProductProperty
	{
		long ScopeId { get; set; }
		long PropertyId { get; set; }
	}

	public class ProductImage : IProductImage
	{
		/// <summary>
		/// 图片
		/// </summary>
		[Required]
		[Image]
		public string Image { get; set; }
		/// <summary>
		/// 标题
		/// </summary>
		[Ignore]
		[StringLength(50,ErrorMessage="标题不能超过50个字")]
		public string Title { get; set; }
	}
	public class ProductDescItem : IProductDescItem
	{
		[Required]
		[Image]
		public string Image { get; set; }
		[Ignore]
		public string Title { get; set; }
	}

	public class ProductContent
	{
		/// <summary>
		/// 产品图片
		/// </summary>
		[Required]
		[Range(1, 5, ErrorMessage = "需要提供1到5张产品图片")]
		[Layout(1)]
		[ArrayLayout(true)]
		public IEnumerable<ProductImage> Images { get; set; }

		/// <summary>
		/// 产品介绍
		/// </summary>
		[Required]
		[Layout(2)]
		public IEnumerable<ProductDescItem> Descs { get; set; }
	}
    [EntityObject]
    public class ProductSpec : IEntityWithId<long>, IEntityWithName
	{
		/// <summary>
		/// ID
		/// </summary>
		[Key]
        [TableVisible]
        [ReadOnly(true)]
        public long Id { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[TableVisible]
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

		/// <summary>
		/// 图片
		/// </summary>
		[Image]
        public string Image { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		[MaxLength(200)]
        public string Desc { get; set; }

		///<title>自动发货规格</title>
		/// <summary>
		/// 卡密类虚拟商品有效
		/// </summary>
		//[EntityIdent("虚拟项目自动发货规格")]
		public long? VIADSpecId { get; set; }
    }
    public class ProductSpecDetail : ProductSpec
    {
        [Ignore]
        public long Order { get; set; }

		/// <summary>
		/// 状态
		/// </summary>
		[TableVisible]
        [Ignore]
        public EntityLogicState ObjectState { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[ReadOnly(true)]
        [Ignore]
        public DateTime CreatedTime { get; set; }

		/// <summary>
		/// 修改时间
		/// </summary>
		[TableVisible]
        [ReadOnly(true)]
        [Ignore]
        public DateTime UpdatedTime { get; set; }

    }
    [EntityObject()]
    public class ProductBase :
		IEntityWithId<long>,
		IEntityWithName
	{
		///<title>ID</title>
		/// <prompt>
		/// 保存后自动产生
		/// </prompt>
		[Key]
		[Layout(1, 1, 10)]
		[ReadOnly(true)]
		[TableVisible(10)]
		public long Id { get; set; }

		/// <title>产品名称</title>
		/// <summary>
		/// 内部跟踪用的产品名称，比如 欧顿手表2016-14361型
		/// </summary>
		/// <order>12</order>
		[Required]
		[StringLength( 100, ErrorMessage = "产品名称不能超过100个字")]
		[Layout(1, 1, 20)]
		[TableVisible(20)]
		public string Name { get; set; }


		/// <title>展示标题</title>
		/// <summary>
		/// 用于前端展示的产品标题，宣传性质，比如“欧顿(OUDUN)手表 男士多功能时尚商务三眼日历运动防水钢带皮带男表 3011黑”
		/// </summary>
		/// <order>13</order>
		[StringLength(100, MinimumLength = 4, ErrorMessage = "标题不能超过100个字")]
		[Layout(1, 1, 30)]
		[Required]
		public string Title { get; set; }


		/// <title>市场价</title>
		/// <summary>
		/// 市场参考价，媒体价格，前端暂未使用
		/// </summary>
		/// <order>11</order>
		[Range(0,9999999,ErrorMessage ="市场价必须在0到1000万之间")]
		[Layout(1, 1, 40,10)]
		[Required]
		[DataType(DataType.Currency)]
		public decimal MarketPrice { get; set; }

		/// <title>售价</title>
		/// <summary>
		/// 实际销售价格
		/// </summary>
		[Range(0, 9999999, ErrorMessage = "售价必须在0到1000万之间")]
		[Layout(1, 1, 40,20)]
		[Required]
		[DataType(DataType.Currency)]
		[TableVisible(30)]
		public decimal Price { get; set; }

		/// <title>主图</title>
		/// <summary>
		///产品大图片，主要用于在产品列表的显示。
		/// </summary>
		[Image]
		[Required]
		[Layout(1, 2)]
		public string Image { get; set; }


		/// <title>虚拟商品</title>
		/// <summary>
		/// 产品是否为虚拟商品，虚拟商品通过卡密方式发货
		/// </summary>
		[Layout(1, 1, 60)]
        public bool IsVirtual { get; set; }

		/// <title>禁止优惠券</title>
		/// <summary>
		/// 不允许使用优惠券
		/// </summary>
        [Layout(1, 1, 60)]
        public bool CouponDisabled { get; set; }

		/// <title>产品发布时间</title>
		/// <summary>
		/// 产品上线时间，可以根据需要设定为任何时间，最新商品列表按此时间排序。
		/// </summary>
		[Layout(1, 1, 60)]
		public DateTime? PublishedTime { get; set; }


		/// <summary>
		/// 产品状态
		/// </summary>
		[Layout(1, 1, 70)]
		[Required]
		[TableVisible(40)]
		public EntityLogicState ObjectState { get; set; }

    }
	public class ProductEditable : ProductBase
	{

		/// <title>产品类型</title>
		/// <summary>
		/// 产品类型主要用于决定产品单位等基本信息，内部管理使用，按实际产品属性选择。
		/// </summary>
		[Required]
		[EntityIdent(typeof(ProductType))]
		[Layout(1, 1, 1)]
		public long TypeId { get; set; }

		/// <title>产品提供人</title>
		/// <summary>
		/// 商品提供人，一般默认即可
		/// </summary>
		//[EntityIdent("产品供应商")]
		[Layout(1, 1, 2)]
		[Required]
		public long OwnerUserId { get; set; }

		/// <title>商品分类</title>
		/// <summary>
		/// 支持多选，用于前端将商品展示在不同区域。
		/// </summary>
		[EntityIdent(typeof(CategoryInternal))]
		[Layout(1,1, 75)]
		public IEnumerable<long> CategoryIds { get; set; }

		[Required]
		[Layout(3)]
		public ProductContent Content { get; set; }


		/// <title>产品规格</title>
		/// <summary>
		/// 注意：1.无规格的产品在发货记录中直接记录产品，有规格的奖品在发货记录中记录产品规格。2.产品规格在开始使用后，不要删除或修改名称(比如把移动改成联通)，否则和发货记录中记录的信息会不服。
		/// </summary>
        [TableRows]
        public IEnumerable<ProductSpecDetail> Specs { get; set; }

		/// <title>自动发货规格</title>
		/// <summary>
		/// 卡密类虚拟奖品有效,如果奖品包含规格，需要针对每个规格进行设置，本项无效
		/// </summary>
		//[EntityIdent("虚拟项目自动发货规格")]
		public long? VIADSpecId { get; set; }

		//[Ignore]
		//public ProductProperty[] Properties { get; set; }
	}
	public class ProductInternal: ProductBase
	{
		/// <summary>
		/// 更新时间
		/// </summary>
		[TableVisible(90)]
		public DateTime UpdatedTime { get; set; }

		public DateTime CreatedTime { get; set; }

		[EntityIdent(typeof(ProductType),nameof(ProductTypeName))]
		public long ProductTypeId { get; set; }

		/// <summary>
		/// 产品类型
		/// </summary>
		[TableVisible(15)]
		public string ProductTypeName { get; set; }
	}

	public class ProductInternalQueryArgument : IQueryArgument<ObjectKey<long>>
	{

		/// <summary>
		/// 产品ID
		/// </summary>
		public ObjectKey<long> Id { get; set; }

		/// <summary>
		/// 产品类型
		/// </summary>
		[EntityIdent(typeof(ProductType))]
		public long? ProductTypeId{get;set;}


		/// <summary>
		/// 更新时间
		/// </summary>
		public DateQueryRange UpdateTime { get; set; }


		/// <summary>
		/// 价格区间
		/// </summary>
		public QueryRange<decimal> Price { get; set; }

		/// <summary>
		/// 产品名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 状态
		/// </summary>
		public EntityLogicState? State { get; set; }

	}
}
