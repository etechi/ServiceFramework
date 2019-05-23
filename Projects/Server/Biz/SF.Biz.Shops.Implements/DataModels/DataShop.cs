using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities.DataModels;
using SF.Sys.Data;

namespace SF.Biz.Shops.DataModels
{
    /// <summary>
    /// 店铺
    /// </summary>
    public class DataShop : DataUIObjectEntityBase
    {
        /// <summary>
        /// 卖家ID
        /// </summary>
        [Index]
        public long SellerId { get; set; }

        [Index]
        public long? AddressId { get; set; }

    }
}
