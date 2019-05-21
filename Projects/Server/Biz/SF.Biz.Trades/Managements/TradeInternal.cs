using SF.Biz.Accounting;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Trades
{
    
    /// <summary>
    /// 交易
    /// </summary>
    public class TradeInternal:TradeBase
	{
        /// <summary>
        /// 根业务
        /// </summary>
        [ReadOnly(true)]
        [EntityIdent(NameField:nameof(BizRootName),WithBizType =true)]
        public string BizRoot { get; set; }

        /// <summary>
        /// 根业务
        /// </summary>
        [TableVisible]
        [Ignore]
        public string BizRootName { get; set; }

        /// <summary>
        /// 父业务
        /// </summary>
        [ReadOnly(true)]
        [EntityIdent(NameField: nameof(BizParentName), WithBizType = true)]
        public string BizParent { get; set; }

        /// <summary>
        /// 父业务
        /// </summary>
        [TableVisible]
        [Ignore]
        public string BizParentName { get; set; }

        /// <summary>
        /// 订单条目
        /// </summary>
        public IEnumerable<TradeItemInternal> Items { get; set; }

    }
}
