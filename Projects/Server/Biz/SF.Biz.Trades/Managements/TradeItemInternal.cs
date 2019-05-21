using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;
using System;
using System.ComponentModel;

namespace SF.Biz.Trades
{
   public class TradeItemInternal :TradeItemBase { 

        /// <summary>
        /// 交易状态
        /// </summary>
        [TableVisible(100)]
        public TradeState State{ get; set; }

        /// <summary>
        /// 完成类型
        /// </summary>
        [TableVisible(110)]
        public TradeEndType EndType { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? EndTime{ get; set; }


        /// <summary>
        /// 买家
        /// </summary>
        [EntityIdent(typeof(User),nameof(BuyerName))]
        public long BuyerId { get; set; }

        /// <summary>
        /// 买家
        /// </summary>
        [Ignore]
        [TableVisible]
        public string BuyerName { get; set; }

        /// <summary>
        /// 交易
        /// </summary>
        [TableVisible]
        [EntityIdent(typeof(Trade),nameof(TradeType))]
        public long TradeId { get; set; }
    }

}
