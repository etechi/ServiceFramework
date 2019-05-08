using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;

namespace SF.Biz.ShoppingCarts.DataModels
{
    /// <summary>
    /// 购物车项目
    /// </summary>
    public class DataShoppingCartItem : DataUIObjectEntityBase
    {
        /// <summary>
        /// 买家ID
        /// </summary>
        [Index("pk",IsUnique =true,Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public long BuyerId { get; set; }

        /// <summary>
        /// 购物车类型
        /// </summary>
        [Index("pk", IsUnique = true, Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(8)]
        public string Type { get; set; }

        /// <summary>
        /// 卖家ID
        /// </summary>
        [Index("pk", IsUnique = true, Order = 3)]
        [Index]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SellerId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [Index("pk", IsUnique = true, Order = 4)]
        [Index]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ItemId { get; set; }

        /// <summary>
        /// SKUID
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Index("pk", IsUnique = true, Order = 5)]
        [Display(Name = "")]
        public long SkuId { get; set; }

        /// <summary>
        /// 卖家标题
        /// </summary>
        public string SellerTitle { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        [Index]        
        public long ProductId { get; set; }


        /// <summary>
        /// 规格
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 售价
        /// </summary>
        public decimal Price { get; set; }


    }
}
