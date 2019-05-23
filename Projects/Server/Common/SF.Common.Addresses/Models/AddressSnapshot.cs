using SF.Sys.Annotations;
using SF.Sys.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Addresses
{
    /// <summary>
    /// 地址快照
    /// </summary>
    public class AddressSnapshot : AddressDetail, IEntityWithId<long>, IEntityWithName
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [ReadOnly(true)]
        public long Id { get; set; }

        /// <summary>
        /// 快照名称
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
    }
}
