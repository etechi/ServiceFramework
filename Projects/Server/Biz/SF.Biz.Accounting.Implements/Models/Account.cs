using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;

namespace SF.Biz.Accounting.DataModels
{
    /// <summary>
    /// 账户
    /// </summary>
	public class DataAccount : DataObjectEntityBase
	{
		[Key]
		[Column(Order = 2)]
        [Display(Name = "账户科目ID")]
        public int AccountTitleId { get; set; }

		[ForeignKey(nameof(AccountTitleId))]
		public DataAccountTitle AccountTitle { get; set; }

        /// <summary>
        /// 转入金额
        /// </summary>
        [Index]
        public decimal Inbound { get; set; }

        /// <summary>
        /// 转出金额
        /// </summary>
        [Index]
        public decimal Outbound { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        [Index]
        public decimal CurValue { get; set; }

    

	}
}
