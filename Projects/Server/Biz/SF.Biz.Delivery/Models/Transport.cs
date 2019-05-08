using SF.Sys.Annotations;
using SF.Sys.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Delivery
{
    /// <summary>
    /// 发货渠道
    /// </summary>
    [EntityObject]
    public class Transport : UIObjectEntityBase
	{

        /// <summary>
        /// 联系人
        /// </summary>
        [TableVisible]
		[StringLength(8)]
		public string ContactName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [TableVisible]
		[StringLength(20)]
		public string ContactPhone { get; set; }

        /// <summary>
        /// 代号
        /// </summary>
        [TableVisible]
		[StringLength(20)]
		public string Ident { get; set; }

    }
}
