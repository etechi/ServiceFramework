using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{
    public class AccountQueryArguments : ObjectQueryArgument
    {
        /// <summary>
        /// 用户
        /// </summary>
        [EntityIdent(typeof(User))]
        public long? OwnerId { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle))]
        public int? TitleId { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateQueryRange UpdatedTime { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public QueryRange<decimal> Amount { get; set; }

        /// <summary>
        /// 转入数额
        /// </summary>
        public QueryRange<decimal> Inbound { get; set; }

        /// <summary>
        /// 转出数额
        /// </summary>
        public QueryRange<decimal> Outbound { get; set; }

    }
    public interface IAccountManager :
        IEntitySource<ObjectKey<long>,Account,AccountQueryArguments>
    {

    }
}
