using SF.Data.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Bizness.Accounting
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
    public class AccountInternal : ObjectEntityBase
    {
        [Display(Name ="用户ID")]
        [TableVisible]
        [Key]
        [EntityIdent("用户",nameof(OwnerName))]
        public int OwnerId { get; set; }

        [Key]
        [EntityIdent("账户类别", nameof(TypeName))]
        [TableVisible]
        [Display(Name = "类别ID")]
        public int TypeId { get; set; }

        [Display(Name = "用户名")]
        [TableVisible]
        [Ignore]
        public string OwnerName { get; set; }

        [Display(Name = "类别")]
        [TableVisible]
        [Ignore]
        public string TypeName { get; set; }

        [Display(Name = "余额")]
        [TableVisible]
        public decimal Amount { get; set; }

        [Display(Name = "转入")]
        [TableVisible]
        public decimal Inbound { get; set; }

        [Display(Name = "转出")]
        [TableVisible]
        public decimal Outbound { get; set; }

        
    }
}
