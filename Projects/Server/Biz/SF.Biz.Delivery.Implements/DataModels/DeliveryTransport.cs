
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using ServiceProtocol.Data.Entity;
using ServiceProtocol.Annotations;

namespace ServiceProtocol.Biz.Delivery.Entity.Models
{
	
	[Table("app_biz_delivery_transport")]
    [TypeDisplay(GroupName = "发货服务", Name = "快递公司")]
    public class DeliveryTransport:
		IObjectWithId<int>
	{
		[Key]
        [Display(Name="ID")]
		public int Id { get; set; }

		[Index]
        [Display(Name = "是否禁用")]
        public bool Disabled { get; set; }

		[Index]
        [Display(Name = "排位")]
        public int Order { get; set; }

		[Required]
        [Display(Name = "名称")]
        [MaxLength(100)]
		public string Name { get; set; }

		[MaxLength(100)]
        [Display(Name = "联系人")]
        public string ContactName { get; set; }

		[MaxLength(100)]
        [Display(Name = "联系电话")]
        public string ContactPhone { get; set; }

		[Required]
		[MaxLength(100)]
        [Display(Name = "代号")]
        public string Ident { get; set; }

		[MaxLength(100)]
        [Display(Name = "网站")]
        public string Site { get; set; }
	}
}
