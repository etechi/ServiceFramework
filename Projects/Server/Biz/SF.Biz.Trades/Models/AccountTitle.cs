using SF.Sys.Annotations;
using SF.Sys.Entities.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Accounting
{
    /// <summary>
    /// 账户科目
    /// </summary>
    public class AccountTitle : ObjectEntityBase
    {
        /// <summary>
        /// 标示
        /// </summary>
        [MaxLength(20)]
        [Required]
        [TableVisible]
        public string Ident { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(20)]
        [Required]
        [TableVisible]
        [EntityTitle]
        public string Title { get; set; }

        /// <summary>
        /// 是否用于结算
        /// </summary>
        [TableVisible]
        public bool SettlementEnabled { get; set; }

        /// <summary>
        /// 结算排序
        /// </summary>
        [TableVisible]
        public int SettlementOrder { get; set; }

        
        [Display(Name ="排位")]
        [TableVisible]
        [Optional]
        public int Order { get; set; }

    }
}
