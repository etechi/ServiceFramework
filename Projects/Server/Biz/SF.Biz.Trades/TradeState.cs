using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Trades
{
	public enum TradeEndType
	{
        /// <summary>
        /// 未完成
        /// </summary>
		InProcessing,                      //订单还在进行
        /// <summary>
        /// 卖方取消
        /// </summary>
        SellerCancelled,                    //卖家取消
        /// <summary>
        /// 卖方未确认
        /// </summary>
        SellerConfirmExpired,               //卖家确认超时
        /// <summary>
        /// 卖方终止
        /// </summary>
        SellerAborted,                      //卖家终止
        /// <summary>
        /// 买方放弃
        /// </summary>
        BuyerCancelledBeforeConfirm,        //买家在确认（支付）前取消
        /// <summary>
        /// 买方取消
        /// </summary>
        BuyerCancelledAfterConfirm,         //买家在确认（支付）后，卖家确认前取消
        /// <summary>
        /// 买方未支付
        /// </summary>
        BuyerConfirmExpired,                //买家确认（支付）超时
        /// <summary>
        /// 买方支付失败
        /// </summary>
        BuyerConfirmError,                //买家确认（支付）失败
        /// <summary>
        /// 买方终止
        /// </summary>
        BuyerAborted,						//买家终止
        /// <summary>
        /// 正常完成
        /// </summary>
        Completed                           //正常结束
    }


    /*
     * 创建订单
     * 买家支付
     * 卖家确认
     * 卖家发货
     * 买家确认
     * 卖家确认
     * 撤销
     */

	/// <summary>
    /// 交易状态
    /// </summary>
	public enum TradeState
	{
      
        /// <summary>
        /// 支付
        /// </summary>
        BuyerConfirm,

        /// <summary>
        /// 待确认
        /// </summary>
        SellerConfirm,
        
        /// <summary>
        /// 待发货
        /// </summary>
        SellerComplete,
        
        /// <summary>
        /// 待收货
        /// </summary>
        BuyerComplete,

        /// <summary>
        /// 卖方收款
        /// </summary>
        SellerSettlement,

        /// <summary>
        /// 已结束
        /// </summary>
        Closed
    }
}
