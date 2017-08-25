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
	public enum RefundState
    {
        [Display(Name = "提交中")]
        Sending,

        [Display(Name ="处理中")]
        Processing,

        [Display(Name = "已完成")]
        Success,

        [Display(Name = "错误")]
        Error
    }

    [EntityObject("账户退款记录")]
    public class RefundRecord : ServiceProtocol.ObjectManager.IDataObject
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
                return Accounting.Ident.CreateRefundIdent(CreatedTime, SrcId.ToString(), Id);
            }
        }

        [TableVisible]
        [Display(Name = "充值编号")]
        [Layout(1, 2)]
        [EntityTitle]
        public string DepositRecordIdent
        {
            get
            {
                return Accounting.Ident.CreateDepositIdent(DepositRecordCreateTime, SrcId.ToString(), DepositRecordId);
            }
        }
        [Ignore]
        public int DepositRecordId { get; set; }

        [TableVisible]
        [Display(Name = "充值时间")]
        public DateTime DepositRecordCreateTime { get; set; }

        [EntityIdent("用户", nameof(SrcName))]
        [Display(Name = "用户")]
        [Layout(2, 1)]
        public int SrcId { get; set; }

        [TableVisible]
        [Display(Name = "用户名")]
        [Ignore]
        public string SrcName { get; set; }

        [EntityIdent("账户科目",nameof(AccountTitle))]
        [Display(Name="科目")]
        [Layout(2, 2)]
        public int AccountTitleId { get; set; }

        [Display(Name = "科目")]
        [Ignore]
        public string AccountTitle { get; set; }

        [TableVisible]
        [Display(Name = "退款开始")]
        [Layout(3, 1)]
        public DateTime CreatedTime { get; set; }

        [TableVisible]
        [Display(Name = "退款结束")]
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
        public RefundState State { get; set; }

        [TableVisible]
        [Display(Name = "标题")]
        [Layout(10)]
        public string Title { get; set; }

        [Display(Name = "操作地址")]
        public string OpAddress { get; set; }

        [Display(Name = "操作设备")]
        public DeviceType OpDevice { get; set; }

        [Display(Name = "支付退款记录")]
        [EntityIdent("支付退款记录")]
        public string RefundRecordId => Ident;

        [Display(Name = "支付平台")]
        [EntityIdent("系统服务",nameof(PaymentPlatformName))]
        public int PaymentPlatformId { get; set; }

        [Display(Name = "支付平台")]
        [TableVisible]
        [Ignore]
        public string PaymentPlatformName { get; set; }

        [Display(Name = "支付描述")]
        public string PaymentDesc { get; set; }

        [Display(Name ="业务来源")]
        [EntityIdent]
        public string TrackEntityIdent { get; set; }

        [Display(Name ="退款原因")]
        [TableVisible]
        public string Reason { get; set; }

        [Display(Name = "错误信息")]
        [TableVisible]
        public string Error { get; set; }

        string IDataObject.Ident => "账户退款记录-" + Id;

        string IDataObject.Name => Title;
    }
}
