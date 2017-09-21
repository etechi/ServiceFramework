using SF.Entities;
using SF.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Products
{
	[EntityObject("商品")]
    public class ItemInternal :
		IEntityWithId<long>
	{
        [Key]
        [ReadOnly(true)]
        [Display(Name ="ID")]
        [TableVisible]
		public long Id { get; set; }
        
        [Ignore]
        public long? SourceItemId { get; set; }

        [Ignore]
        [Display(Name ="产品")]
        [EntityIdent(typeof(ProductInternal),"Title")]
        [TableVisible]
        public long ProductId { get; set; }

        [Display(Name = "图片")]
        [Image]
        public string Image { get; set; }

        [Display(Name = "标题")]
        [TableVisible]
        public string Title { get; set; }

        [Display(Name ="价格")]
        [TableVisible]
        public decimal? Price { get; set; }

        [Display(Name ="卡密")]
        [TableVisible]
        public bool IsVirtual { get; set; }
	}
    
	public class ItemEditable :
		IEntityWithId<long>
	{
		public long Id { get; set; }
		public long SellerId { get; set; }
		public long? SourceItemId { get; set; }
		public long ProductId { get; set; }
		public decimal? Price { get; set; }
		public string Title { get; set; }
		public string Image { get; set; }
	}

}
