
using ServiceProtocol.Annotations;
using ServiceProtocol.Data.Entity;
using ServiceProtocol.ObjectManager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Biz.Delivery.Entity.Models
{
	[Table("app_biz_delivery_address")]
    [TypeDisplay(GroupName = "发货服务", Name = "发货地址")]
    public class DeliveryAddress<TUserKey> :
		IObjectWithId<int>
		where TUserKey : IEquatable<TUserKey>
	{
		[Key]
        [Display(Name ="ID")]
		public int Id { get; set; }

		[Index]
        [Display(Name = "所有者ID")]
        public TUserKey OwnerId { get; set; }

        [Display(Name = "是否为默认地址")]
        public bool IsDefaultAddress { get; set; }

		[MaxLength(100)]
        [Display(Name = "联系人")]
        public string ContactName { get; set; }
		[MaxLength(20)]
        [Display(Name = "联系人电话")]
        public string ContactPhoneNumber { get; set; }
        [Display(Name = "电话是否已确认")]
        public bool PhoneNumberVerified { get; set; }
		
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
		[MaxLength(200)]
        [Display(Name = "街道")]
        public string Address { get; set; }

		[MaxLength(20)]
        [Display(Name = "邮编",Description ="暂未使用")]
        public string ZipCode { get; set; }

	}

}
