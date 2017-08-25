using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Biz.Accounting
{
    [EntityObject("账户转账记录")]
    public class TransferRecord
	{
        [Key]
        [Display(Name ="Id")]
        [TableVisible]
        [Layout(1)]
        public int Id { get; set; }

        [Display(Name = "源用户")]
        [EntityIdent("用户", nameof(SrcName))]
        [Layout(2,1)]
        public int SrcId { get; set; }

        [Display(Name = "源用户",Description = "源用户为空为系统奖励操作")]
        [Ignore]
        [TableVisible]
        public string SrcName { get; set; }


        [Display(Name = "源科目")]
        [EntityIdent("账户科目", nameof(SrcTitleName))]
        [Layout(2, 2)]
        public int SrcTitleId { get; set; }

        [Display(Name = "源科目")]
        [Ignore]
        [TableVisible]
        public string SrcTitleName { get; set; }


        [Display(Name = "目标用户", Description ="目标用户为空为系统扣款操作")]
        [EntityIdent("用户", nameof(DstName))]
        [Layout(3,1)]
        public int DstId { get; set; }

        [Display(Name = "目标用户")]
        [Ignore]
        [TableVisible]
        public string DstName { get; set; }


        [Display(Name = "目标科目")]
        [EntityIdent("账户科目", nameof(DstTitleName))]
        [Layout(3, 2)]
        public int DstTitleId { get; set; }

        [Display(Name = "目标科目")]
        [Ignore]
        [TableVisible]
        public string DstTitleName { get; set; }


        [Display(Name = "时间")]
        [TableVisible]
        [Layout(4, 1)]
        public DateTime Time { get; set; }

        [Display(Name = "数额")]
        [TableVisible]
        [Layout(4, 2)]
        public decimal Amount { get; set; }

        [Display(Name = "新源数额")]
        [TableVisible]
        [Layout(4, 3)]
        public decimal SrcCurValue { get; set; }

        [Display(Name = "新目标数额")]
        [TableVisible]
        [Layout(4, 4)]
        public decimal DstCurValue { get; set; }

        [Display(Name = "标题")]
        [TableVisible]
        [Layout(5)]
        public string Title { get; set; }

        [Display(Name = "操作地址")]
        public string OpAddress { get; set; }

        [Display(Name = "操作设备")]
        public DeviceType OpDevice { get; set; }

        [Display(Name = "业务来源")]
        [EntityIdent]
        public string TrackEntityIdent { get; set; }

    }
}
