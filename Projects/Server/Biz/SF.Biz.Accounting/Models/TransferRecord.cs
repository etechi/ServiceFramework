using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Entities.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Accounting
{
    /// <summary>
    /// 账户转账记录
    /// </summary>
    [EntityObject]

    public class TransferRecord : EventEntityBase
	{


        /// <summary>
        /// 源用户
        /// </summary>
        [EntityIdent(typeof(User), nameof(SrcName))]
        public long SrcId { get; set; }

        ///<title>源用户</title>
        /// <summary>
        /// 源用户为空为系统奖励操作
        /// </summary>
        [Ignore]
        [TableVisible]
        public string SrcName { get; set; }


        /// <summary>
        /// 源科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle),nameof(SrcTitleName))]

        public long SrcTitleId { get; set; }

        /// <summary>
        /// 源科目
        /// </summary>
        [Ignore]
        [TableVisible]
        public string SrcTitleName { get; set; }

        ///<title>目标用户</title>
        /// <summary>
        /// 目标用户为空为系统扣款操作
        /// </summary>
        [EntityIdent(typeof(User), nameof(DstName))]
        [Layout(3,1)]
        public long DstId { get; set; }

        /// <summary>
        /// 目标用户
        /// </summary>
        [Ignore]
        [TableVisible]
        public string DstName { get; set; }


        /// <summary>
        /// 目标科目
        /// </summary>
        [EntityIdent(typeof(AccountTitle), nameof(DstTitleName))]
        public long DstTitleId { get; set; }

        /// <summary>
        /// 目标科目
        /// </summary>
        [Ignore]
        [TableVisible]
        public string DstTitleName { get; set; }



        /// <summary>
        /// 数额
        /// </summary>
        [TableVisible]
        public decimal Amount { get; set; }

        /// <summary>
        /// 新源数额
        /// </summary>
        [TableVisible]
        public decimal SrcCurValue { get; set; }

        /// <summary>
        /// 新目标数额
        /// </summary>
        [TableVisible]
        public decimal DstCurValue { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [TableVisible]
        public string Title { get; set; }

        /// <summary>
        /// 操作地址
        /// </summary>
        public string OpAddress { get; set; }

        /// <summary>
        /// 操作设备
        /// </summary>
        public ClientDeviceType OpDevice { get; set; }

        /// <summary>
        /// 根业务
        /// </summary>
        [EntityIdent(NameField: nameof(BizRootName), WithBizType = true)]
        [ReadOnly(true)]
        public string BizRoot { get; set; }

        /// <summary>
        /// 根业务
        /// </summary>
        [Ignore]
        [TableVisible]
        public string BizRootName { get; set; }

        /// <summary>
        /// 父业务
        /// </summary>
        [EntityIdent(NameField: nameof(BizParentName), WithBizType = true)]
        [ReadOnly(true)]
        public string BizParent { get; set; }

        /// <summary>
        /// 父业务
        /// </summary>
        [Ignore]
        [TableVisible]
        public string BizParentName { get; set; }

    }
}
