using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;
using SF.Sys.Clients;

namespace SF.Biz.Trades.DataModels
{
    /// <summary>
    /// 交易
    /// </summary>
    public class DataTrade : DataObjectEntityBase
	{
        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        ///<title>结算金额</title>
        /// <summary>
        /// 结算金额，折扣说明在desc表
        /// </summary>
        public decimal TotalSettlementAmount { get; set; }

        /// <summary>
        /// 买方结算所得
        /// </summary>
        public decimal BuyerAmount { get; set; }
        /// <summary>
        /// 卖方结算所得
        /// </summary>
        public decimal TotalSellerAmount { get; set; }
        /// <summary>
        /// 平台结算所得
        /// </summary>
        public decimal PlatformAmount { get; set; }

		[Index("seller_state_createtime",Order =1)]
		[Index("seller_endtype_createtime", Order = 1)]
        [Display(Name = "卖家ID")]
        public long SellerId { get; set; }

		[Index("buyer_state_createtime", Order = 1)]
		[Index("buyer_endtype_createtime", Order = 1)]
        [Display(Name = "买家ID")]
        public long BuyerId { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
		[Index("seller_state_createtime", Order = 2)]
		[Index("buyer_state_createtime", Order = 2)]
        public TradeState State { get; set; }

        /// <summary>
        /// 交易结束类型
        /// </summary>
		[Index("seller_endtype_createtime", Order = 2)]
		[Index("buyer_endtype_createtime", Order = 2)]
        public TradeEndType EndType { get; set; }

        /// <summary>
        /// 交易结束原因
        /// </summary>
		[MaxLength(200)]
        public string EndReason { get; set; }

        /// <summary>
        /// 交易结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 交易创建时间
        /// </summary>
        [Index]
		[Index("buyer_state_createtime", Order = 3)]
		[Index("buyer_endtype_createtime", Order = 3)]
		[Index("seller_state_createtime", Order = 3)]
		[Index("seller_endtype_createtime", Order = 3)]
        public override DateTime CreatedTime { get; set; }

        /// <summary>
        /// 最后状态变更时间
        /// </summary>
        public DateTime LastStateTime { get; set; }

        /// <summary>
        /// 状态过期时间
        /// </summary>
        public DateTime? StateExpires { get; set; }


		[InverseProperty(nameof(DataTradeItem.Trade))]
		public ICollection<DataTradeItem> Items {get;set;}


        /// <summary>
        /// 交易生成设备
        /// </summary>
        public ClientDeviceType OpDevice { get; set; }

        /// <summary>
        /// 交易生成地址
        /// </summary>
        [MaxLength(20)]
		public string OpAddress { get; set; }

        /// <summary>
        /// 折扣跟踪标识
        /// </summary>
        [MaxLength(100)]
        public string DiscountEntityIdent { get; set; }

        /// <summary>
        /// 折扣数量
        /// </summary>
        public int DiscountEntityCount { get; set; }

        /// <summary>
        /// 交易充值记录ID
        /// </summary>
        public long? DepositRecordId { get; set; }
        /// <summary>
        /// 交易退款记录ID
        /// </summary>
        public long? RefundRecordId { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        [Index]
        public TradeType TradeType { get; set; }

        /// <summary>
        /// 卖家名称
        /// </summary>
        [MaxLength(200)]
        public string SellerName { get; set; }

        /// <summary>
        /// 买家名称
        /// </summary>
		[MaxLength(200)]
        public string BuyerName { get; set; }

        /// <summary>
        /// 折扣说明
        /// </summary>
        [MaxLength(200)]
        public string DiscountDesc { get; set; }


        /// <summary>
        /// 买家备注
        /// </summary>
        [MaxLength(200)]
        public string BuyerRemarks { get; set; }

        /// <summary>
        /// 卖家备注
        /// </summary>
		[MaxLength(200)]
        public string SellerRemarks { get; set; }

        /// <summary>
        /// 发货地址
        /// </summary>
		[MaxLength(20)]
        public string DeliveryAddressId { get; set; }

        public void SyncItemsState(IDataContext ctx)
        {
            if(Items!=null)
                foreach(var it in Items)
                {
                    it.State = State;
                    it.EndTime = EndTime;
                    it.LastStateTime = LastStateTime;
                    it.EndType = EndType;
                    ctx.Update(it);
                }
        }
        public void CalcItemsApportionAmount(IDataContext ctx=null)
        {
            var sum = 0m;
            var count = Items.Count;
            var idx = 0;
            foreach(var it in Items)
            {
                idx++;
                if (idx == count)
                    it.ApportionAmount = TotalSettlementAmount - sum;
                else
                {
                    it.ApportionAmount = Math.Floor(TotalSettlementAmount * it.SettlementAmount * 100m / TotalAmount) * 0.01m;
                    sum += it.ApportionAmount;
                }
                if(ctx!=null)
                    ctx.Update(it);
            }
        }
    }
}
