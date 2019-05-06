using ServiceProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{
    public class BalanceNotEnough : PublicInvalidOperationException
    {
        public decimal Current { get; }
        public decimal Required { get; }
        public BalanceNotEnough(decimal Current, decimal Required) : base("余额不足") {
            this.Current = Current;
            this.Required = Required;
        }
    }
    public class Account
    {
		public decimal Inbound { get; set; }
		public decimal Outbound { get; set; }
	}

    [EntityObject("账户")]
    public class AccountInternal
    {
        [Display(Name ="用户ID")]
        [TableVisible]
        [Key]
        [EntityIdent("用户",nameof(OwnerName))]
        public int OwnerId { get; set; }

        [Key]
        [EntityIdent("账户科目", nameof(TitleName))]
        [TableVisible]
        [Display(Name = "科目ID")]
        public int TitleId { get; set; }

        [Display(Name = "用户名")]
        [TableVisible]
        [Ignore]
        public string OwnerName { get; set; }

        [Display(Name = "科目")]
        [TableVisible]
        [Ignore]
        public string TitleName { get; set; }

        [Display(Name = "余额")]
        [TableVisible]
        public decimal Amount { get; set; }

        [Display(Name = "转入")]
        [TableVisible]
        public decimal Inbound { get; set; }

        [Display(Name = "转出")]
        [TableVisible]
        public decimal Outbound { get; set; }

        [Display(Name = "更新时间")]
        [TableVisible]
        public DateTime UpdatedTime { get; set; }
        
    }
}
