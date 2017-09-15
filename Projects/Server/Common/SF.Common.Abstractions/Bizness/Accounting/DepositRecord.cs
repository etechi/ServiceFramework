using ServiceProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;

namespace ServiceProtocol.Biz.Accounting
{
	public enum DepositState
	{
        [Display(Name ="处理中")]
        Processing,

        [Display(Name = "已充值未退款")]
        Completed,

        [Display(Name = "失败")]
        Failed,

        [Display(Name = "新建")]
        [Ignore]
        New,

        [Display(Name = "退款中")]
        Refunding,

        [Display(Name = "已退款")]
        Refunded,

        [Display(Name ="已处理")]
        [Ignore]
        AfterProcessing,

        [Display(Name = "已充值含退款")]
        
        CompletedWithRefund
    }

    [EntityObject("账户充值记录")]
    public class DepositRecord : ServiceProtocol.ObjectManager.IDataObject
	{
        [Key]
        [TableVisible]
        [Display(Name ="ID")]
        [Layout(1,1)]
		public int Id { get; set; }

        [TableVisible]
        [Display(Name ="编号")]
        [Layout(1, 2)]
        [EntityTitle]
        public string Ident
        {
            get
            {
                return Accounting.Ident.CreateDepositIdent(CreatedTime, DstId.ToString(), Id);
            }
        }

        [EntityIdent("用户", nameof(DstName))]
        [Display(Name = "用户")]
        [Layout(2, 1)]
        public int DstId { get; set; }

        [TableVisible]
        [Display(Name = "用户名")]
        [Ignore]
        public string DstName { get; set; }

        [EntityIdent("账户科目",nameof(AccountTitle))]
        [Display(Name="科目")]
        [Layout(2, 2)]
        public int AccountTitleId { get; set; }

        [Display(Name = "科目")]
        [Ignore]
        [TableVisible]
        public string AccountTitle { get; set; }

        [TableVisible]
        [Display(Name = "开始时间")]
        [Layout(3, 1)]
        public DateTime CreatedTime { get; set; }

        [TableVisible]
        [Display(Name = "结束时间")]
        [Layout(3, 2)]
        public DateTime? CompletedTime { get; set; }

        [TableVisible]
        [Display(Name = "金额")]
        [Layout(4, 1)]
        public decimal Amount { get; set; }

        [TableVisible]
        [Display(Name = "余额")]
        [Layout(4, 1)]
        public decimal? CurValue { get; set; }

        [TableVisible]
        [Display(Name = "状态")]
        [Layout(4, 2)]
        public DepositState State { get; set; }

        [TableVisible]
        [Display(Name = "标题")]
        [Layout(10)]
        public string Title { get; set; }

        [TableVisible]
        [Display(Name = "操作地址")]
        public string OpAddress { get; set; }

        [TableVisible]
        [Display(Name = "操作设备")]
        public DeviceType OpDevice { get; set; }

        [Display(Name = "支付收款记录")]
        [EntityIdent("支付收款记录")]
        public string CollectRecordId => Ident;

        [Display(Name = "支付平台")]
        [EntityIdent("系统服务",nameof(PaymentPlatformName))]
        public int PaymentPlatformId { get; set; }

        [Display(Name = "支付平台")]
        [Ignore]
        [TableVisible]
        public string PaymentPlatformName { get; set; }

        [Display(Name = "支付描述")]
        public string PaymentDesc { get; set; }

        [Display(Name ="业务来源")]
        [EntityIdent]
        public string TrackEntityIdent { get; set; }

        [Display(Name = "错误信息")]
        public string Error { get; set; }

        [Display(Name = "退款申请金额")]
        public decimal RefundRequest { get; set; }
        [Display(Name = "退款成功金额")]
        public decimal RefundSuccess { get; set; }
        [Display(Name = "最后退款申请时间")]
        public DateTime? LastRefundRequestTime { get; set; }
        [Display(Name = "最后退款成功时间")]
        public DateTime? LastRefundSuccessTime { get; set; }
        [Display(Name = "最后退款原因")]
        public string LastRefundReason { get; set; }

        [Ignore]
        string IDataObject.Ident => "账户充值记录-" + Id;

        [Ignore]
        string IDataObject.Name => Title;
    }
}
