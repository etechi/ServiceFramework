using SF.Data;
using SF.Data.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Bizness.Accounting
{

	[EntityObject("账户类别")]
    public class AccountType : ObjectEntityBase
    {
        [MaxLength(20)]
        [Required]
        [Comment(Name = "标识")]
        [TableVisible]
        public string Ident { get; set; }

        [Display(Name = "是否用于结算")]
        [TableVisible]
        public bool SettlementEnabled { get; set; }

        [Comment(Name = "结算排序")]
        [TableVisible]
        public int SettlementOrder { get; set; }

        [Comment(Name ="排位")]
        [TableVisible]
        [Optional]
        public int Order { get; set; }
    }
}
