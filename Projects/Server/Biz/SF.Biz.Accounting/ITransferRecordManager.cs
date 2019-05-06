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
    public class TransferRecordQueryArguments :QueryArgument
    {

        /// <summary>
        /// 源用户
        /// </summary>
        [EntityIdent(typeof(User))]
        public long? SrcId { get; set; }

        /// <summary>
        /// 源科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle))]
        public long? SrcTitleId { get; set; }

        /// <summary>
        /// 目标用户
        /// </summary>
        [EntityIdent(typeof(User))]
        public long? DstId { get; set; }

        /// <summary>
        /// 目标科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle))]
        public long? DstTitleId { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateQueryRange Time { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public QueryRange<decimal> Amount { get; set; }
    }
    public interface ITransferRecordManager :
        IEntitySource<ObjectKey<long>,TransferRecord,AccountQueryArguments>
    {

    }
}
