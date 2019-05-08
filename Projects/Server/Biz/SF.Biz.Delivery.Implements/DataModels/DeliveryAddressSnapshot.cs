using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ServiceProtocol.Data.Entity;
using ServiceProtocol.Annotations;

namespace ServiceProtocol.Biz.Delivery.Entity.Models
{
	[Table("app_biz_delivery_addr_snapshot")]
    [TypeDisplay(GroupName = "发货服务", Name = "地址快照")]
    public class DeliveryAddressSnapshot
	{
		[Key]
        [Display(Name ="ID")]
		public int Id { get; set; }
		
		[Index(IsUnique =true)]
		[Required]
		[MaxLength(30)]
        [Display(Name = "哈希码")]
        public string Hash { get; set; }

		[Required]
		[MaxLength(30)]
        [Display(Name = "联系人")]
        public string ContactName { get; set; }

		[Required]
		[MaxLength(30)]
        [Display(Name = "联系电话")]
        public string ContactPhoneNumber { get; set; }

		[Index]
        [Display(Name = "地址位置ID")]
        public int LocationId { get; set; }

		[ForeignKey(nameof(LocationId))]
		public DeliveryLocation Location { get; set; }

		[Required]
		[MaxLength(200)]
        [Display(Name = "地址位置名")]
        public string LocationName { get; set; }

		[Required]
		[MaxLength(20)]
        [Display(Name = "邮编")]
        public string ZipCode { get; set; }

		[Required]
		[MaxLength(200)]
        [Display(Name = "街道")]
        public string Address { get; set; }

	}
}
