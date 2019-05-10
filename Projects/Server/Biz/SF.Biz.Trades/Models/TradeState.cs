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
        /// 买方终止
        /// </summary>
        BuyerAborted,						//买家终止
        /// <summary>
        /// 正常完成
        /// </summary>
        Completed                           //正常结束
    }
	
	public enum TradeState
	{
        /// <summary>
        /// 无效
        /// </summary>
        Invalid,

        /*
		1: 订单已创建，等待卖家或买家开始订单
		买家付款
			>> WaitSellerConfirm

		timeout 24h	
			>> Closed		end_type = BuyerConfirmExpired

		买家取消
			>> Closed		end_type = BuyerCancelledBeforeConfirm
		
		卖家取消
			@tourist 预约已被拒绝
			>> Closed		end_type = SellerCancelled
		*/

        /// <summary>
        /// 新订单
        /// </summary>
        Created,


        /*
		2: 买家已经确认开始（支付完成），等待卖家确认

		卖家确认
			>> Established
		
		timeout 24 hours
			>> WaitSettlement end_type =  SellerCancelled
		
		买家取消							
			>> WaitSettlement end_type = BuyerCancelledAfterConfirm

		卖家拒绝
			>> WaitSettlement end_type =  SellerCancelled
		*/
        /// <summary>
        /// 待卖方确认
        /// </summary>
        WaitSellerConfirm,

        /*
		3: 订单成立，卖家已接受预定

		卖家结束服务，等待买家确认	
			>> WaitBuyerComplete
		
		买家终止订单
			>> WaitSettlement	end_type = BuyerAbort

		卖家终止订单
			>> WaitSettlement end_type = SellerAbort
		*/
        /// <summary>
        /// 待发货
        /// </summary>
        Established,

        /*
		4: 卖家结束服务，等待买家确认
		买家确认
			>> WaitSettlement	end_type = Completed
		timeout 7 days
			>> WaitSettlement	end_type = Completed
		*/
        /// <summary>
        /// 待收货
        /// </summary>
        WaitBuyerComplete,

        /*
		5: 等待平台进行结算,根据订单情况，可能自动跳转到后续几个状态
		买家结算完成(向买家退款，或打款）
			>> WaitSellerSettlement

		卖家结算完成(向卖家打款）
			>> WaitBuyerSettlement
		*/
        /// <summary>
        /// 待双方结算
        /// </summary>
        WaitSettlement,

        /*
		6: 等待平台进行买家结算
		买家结算完成
			>> closed
		*/
        /// <summary>
        /// 待买方结算
        /// </summary>
        WaitBuyerSettlement,

        /*
		7: 等待平台进行卖家结算
		卖家结算完成
			>> Closed
		*/
        /// <summary>
        /// 待卖方结算
        /// </summary>
        WaitSellerSettlement,

        /*
		8: 订单关闭
		*/
        /// <summary>
        /// 已结束
        /// </summary>
        Closed
    }
}
