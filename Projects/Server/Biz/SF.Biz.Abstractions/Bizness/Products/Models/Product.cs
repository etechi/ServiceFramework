using SF.Entities;
using SF.Metadata;
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
		[Required]
		[Display(Name = "图片")]
		[Image]
		public string Image { get; set; }

		[Ignore]
		[Display(Name ="标题")]
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
		[Required]
		[Display(Name = "产品图片")]
		[Range(1, 5, ErrorMessage = "需要提供1到5张产品图片")]
		[Layout(1)]
		[ArrayLayout(true)]
		public IEnumerable<ProductImage> Images { get; set; }

		[Required]
		[Display(Name = "产品介绍")]
		[Layout(2)]
		public IEnumerable<ProductDescItem> Descs { get; set; }
	}
    [EntityObject]
    public class ProductSpec : IEntityWithId<long>
    {
        [Key]
        [Display(Name = "ID")]
        [TableVisible]
        [ReadOnly(true)]
        public long Id { get; set; }

        [Display(Name = "名称")]
        [TableVisible]
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "图片")]
        [Image]
        public string Image { get; set; }

        [Display(Name = "描述")]
        [MaxLength(200)]
        public string Desc { get; set; }


		[Display(Name = "自动发货规格", Description = "卡密类虚拟商品有效")]
		//[EntityIdent("虚拟项目自动发货规格")]
		public long? VIADSpecId { get; set; }
    }
    public class ProductSpecDetail : ProductSpec
    {
        [Ignore]
        public long Order { get; set; }

		[Display(Name = "状态")]
        [TableVisible]
        [Ignore]
        public EntityLogicState ObjectState { get; set; }

        [Display(Name = "创建时间")]
        [ReadOnly(true)]
        [Ignore]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "修改时间")]
        [TableVisible]
        [ReadOnly(true)]
        [Ignore]
        public DateTime UpdatedTime { get; set; }

    }
    [EntityObject()]
    public class ProductBase :
		IEntityWithId<long>
	{
		[Key]
		[Display(Name = "ID",Order =101,Description ="保存后自动产生")]
		[Layout(1, 1, 10)]
		[ReadOnly(true)]
		[TableVisible(10)]
		public long Id { get; set; }

		[Required]
		[Display(Name = "产品名称", Order = 12,Description = "内部跟踪用的产品名称，比如 欧顿手表2016-14361型")]
		[StringLength( 100, ErrorMessage = "产品名称不能超过100个字")]
		[Layout(1, 1, 20)]
		[TableVisible(20)]
		public string Name { get; set; }


		[Display(Name = "展示标题", Order = 13,Description = "用于前端展示的产品标题，宣传性质，比如“欧顿(OUDUN)手表 男士多功能时尚商务三眼日历运动防水钢带皮带男表 3011黑”")]
		[StringLength(100, MinimumLength = 4, ErrorMessage = "标题不能超过100个字")]
		[Layout(1, 1, 30)]
		[Required]
		public string Title { get; set; }

		[Display(Name ="市场价", Order = 11,Description ="市场参考价，媒体价格，前端暂未使用")]
		[Range(0,9999999,ErrorMessage ="市场价必须在0到1000万之间")]
		[Layout(1, 1, 40,10)]
		[Required]
		[DataType(DataType.Currency)]
		public decimal MarketPrice { get; set; }

		[Display(Name = "售价", Description = "实际销售价格")]
		[Range(0, 9999999, ErrorMessage = "售价必须在0到1000万之间")]
		[Layout(1, 1, 40,20)]
		[Required]
		[DataType(DataType.Currency)]
		[TableVisible(30)]
		public decimal Price { get; set; }

		[Image]
		[Required]
		[Display(Name = "主图",Description ="产品大图片，主要用于在产品列表的显示。")]
		[Layout(1, 2)]
		public string Image { get; set; }


        [Layout(1, 1, 60)]
        [Display(Name = "虚拟商品", Description = "产品是否为虚拟商品，虚拟商品通过卡密方式发货")]
        public bool IsVirtual { get; set; }

        [Display(Name = "禁止优惠券", Description = "不允许使用优惠券")]
        [Layout(1, 1, 60)]
        public bool CouponDisabled { get; set; }

        [Layout(1, 1, 60)]
		[Display(Name = "产品发布时间",Description ="产品上线时间，可以根据需要设定为任何时间，最新商品列表按此时间排序。")]
		public DateTime? PublishedTime { get; set; }


		[Layout(1, 1, 70)]
		[Display(Name = "产品状态")]
		[Required]
		[TableVisible(40)]
		public EntityLogicState ObjectState { get; set; }

    }
	public class ProductEditable : ProductBase
	{
		[Required]
		[Display(Name="产品类型",Order =1,Description ="产品类型主要用于决定产品单位等基本信息，内部管理使用，按实际产品属性选择。")]
		[EntityIdent(typeof(ProductType))]
		[Layout(1, 1, 1)]
		public long TypeId { get; set; }

		[Display(Name = "产品提供人",Description = "商品提供人，一般默认即可")]
		//[EntityIdent("产品供应商")]
		[Layout(1, 1, 2)]
		[Required]
		public long OwnerUserId { get; set; }

		[EntityIdent(typeof(CategoryInternal))]
		[Layout(1,1, 75)]
		[Display(Name="商品分类",Description ="支持多选，用于前端将商品展示在不同区域。")]
		public IEnumerable<long> CategoryIds { get; set; }

		[Required]
		[Layout(3)]
		public ProductContent Content { get; set; }


        [Display(Name = "产品规格", Description = "注意：1.无规格的产品在发货记录中直接记录产品，有规格的奖品在发货记录中记录产品规格。2.产品规格在开始使用后，不要删除或修改名称(比如把移动改成联通)，否则和发货记录中记录的信息会不服。")]
        [TableRows]
        public IEnumerable<ProductSpecDetail> Specs { get; set; }

		[Display(Name = "自动发货规格", Description = "卡密类虚拟奖品有效,如果奖品包含规格，需要针对每个规格进行设置，本项无效")]
		//[EntityIdent("虚拟项目自动发货规格")]
		public long? VIADSpecId { get; set; }

		//[Ignore]
		//public ProductProperty[] Properties { get; set; }
	}
	public class ProductInternal: ProductBase
	{
		[Display(Name = "更新时间")]
		[TableVisible(90)]
		public DateTime UpdatedTime { get; set; }

		public DateTime CreatedTime { get; set; }

		[EntityIdent(typeof(ProductType),nameof(ProductTypeName))]
		public long ProductTypeId { get; set; }

		[Display(Name = "产品类型")]
		[TableVisible(15)]
		public string ProductTypeName { get; set; }
	}

	public class ProductInternalQueryArgument : IQueryArgument<ObjectKey<long>>
	{
		[Display(Name = "产品ID")]
		public ObjectKey<long> Id { get; set; }

		[Display(Name = "产品类型")]
		[EntityIdent(typeof(ProductType))]
		public long? ProductTypeId{get;set;}


		[Display(Name = "更新时间")]
		public DateQueryRange UpdateTime { get; set; }

		[Display(Name = "价格区间")]
		public QueryRange<decimal> Price { get; set; }

		[Display(Name = "产品名称")]
		public string Name { get; set; }

		[Display(Name ="状态")]
		public EntityLogicState? State { get; set; }

	}
}
