using SF.Sys;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;
using System;
using System.ComponentModel.DataAnnotations;

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
        public decimal CurValue { get; set; }
	}

    /// <summary>
    /// 账户
    /// </summary>
    [EntityObject]
    public class AccountInternal :ObjectEntityBase
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [TableVisible]
        [Key]
        [EntityIdent(typeof(User),nameof(OwnerName))]
        public override long Id { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Key]
        [EntityIdent(typeof(AccountTitle), nameof(TitleName))]
        [TableVisible]
        public long AccountTitleId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [TableVisible]
        [Ignore]
        public string OwnerName { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [TableVisible]
        [Ignore]
        public string TitleName { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        [TableVisible]
        public decimal Amount { get; set; }

        /// <summary>
        /// 转入
        /// </summary>
        [TableVisible]
        public decimal Inbound { get; set; }

        /// <summary>
        /// 转出
        /// </summary>
        [TableVisible]
        public decimal Outbound { get; set; }

       
        
    }
}
