using SF.Sys.Data;
using SF.Sys.Entities.DataModels;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Accounting.DataModels
{
    /// <summary>
    /// 账户科目
    /// </summary>
    public class DataAccountTitle : DataObjectEntityBase
	{

        /// <summary>
        /// 科目标识
        /// </summary>
        [Required]
		[MaxLength(20)]
		[Index(IsUnique =true)]
        public string Ident { get; set; }

        /// <summary>
        /// 科目标题
        /// </summary>
		[MaxLength(100)]
		[Required]
        public string Title { get; set; }


        /// <summary>
        /// 排位
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 是否用于结算
        /// </summary>
        public bool SettlementEnabled { get; set; }

        /// <summary>
        /// 结算排位
        /// </summary>
        public int SettlementOrder { get; set; }
    }
}
