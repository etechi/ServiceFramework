using SF.Entities;
using SF.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Products
{
	[EntityObject]
    public class CategoryInternal:
		IEntityWithId<long>
	{
		[Key]
		[Display(Name = "ID", Description  = "保存后自动产生")]
		[ReadOnly(true)]
		[TableVisible]
        [Layout(1)]
        public long Id { get; set; }

		[TableVisible]
		[Required]
		[Display(Name = "名称", Description="内部管理使用，比如：2016夏季促销商品")]
		[StringLength(100)]
        [Layout(2)]
        public string Name { get; set; }

		[Required]
		[Display(Name = "标题", Description = "用于前端显示，比如：火热夏季专辑")]
		[TableVisible]
		[StringLength(100)]
        [Layout(3)]
        public string Title { get; set; }



        [Display(Name = "销售人员", Description = "一般默认即可")]
        //[EntityIdent("产品供应商")]
        [Required]
        [Layout(4)]
        public long SellerId { get; set; }

        [EntityIdent(typeof(CategoryInternal), nameof(ParentName), IsTreeParentId = true, ScopeField = nameof(SellerId))]
		[Display(Name = "父目录")]
        [Layout(5)]
        public long? ParentId { get; set; }

		[Display(Name = "父目录")]
		[Ignore]
		[TableVisible]
        [Layout(6)]
        public string ParentName { get; set; }

		
		[TableVisible]
		[Display(Name = "标签",Description ="用于控制前端显示的标签，一般留空即可")]
		[StringLength(50)]
        [Layout(7)]
        public string Tag { get; set; }

		[StringLength(100)]
		[Display(Name = "描述",Description = "商品分类的描述，供前端显示使用")]
        [Layout(8)]
        public string Description { get; set; }

		[Image]
		[Display(Name = "图片", Description = "商品分类的大图片，供前端显示使用")]
        [Layout(9)]
        public string Image { get; set; }

		[Image]
		[Display(Name = "图标", Description = "商品分类的小图片，供前端显示使用")]
        [Layout(10)]
        public string Icon { get; set; }

        [Display(Name = "广告图", Description = "PC栏目页面广告图")]
        [Layout(11)]
        [Image]
        [MaxLength(200)]
        public string BannerImage { get; set; }

        [Display(Name = "广告图链接", Description = "PC栏目页面广告图链接")]
        [Layout(12)]
        [MaxLength(200)]
        public string BannerUrl { get; set; }

        [Display(Name = "移动站广告图", Description = "移动站栏目页面广告图")]
        [Layout(13)]
        [Image]
        [MaxLength(200)]
        public string MobileBannerImage { get; set; }

        [Display(Name = "移动站广告图链接", Description = "移动栏目页面广告图链接")]
        [Layout(14)]
        [MaxLength(200)]
        public string MobileBannerUrl { get; set; }


        [Display(Name = "列表排位",Description ="商品分类在分类列表中的排位，小的在前")]
		[TableVisible]
        [Optional]
        public int Order { get; set; }

		[Display(Name = "对象状态")]
		[TableVisible]
		[Required]
		public EntityLogicState ObjectState { get; set; }

		[Ignore]
		public CategoryInternal[] Children { get; set; }

		[Ignore]
		public long[] Items { get; set; }
    }

}
