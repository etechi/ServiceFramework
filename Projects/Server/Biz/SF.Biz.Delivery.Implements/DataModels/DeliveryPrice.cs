using ServiceProtocol.Biz.Delivery;
using ServiceProtocol.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Biz.Delivery.Entity.Models
{
	
	[Table("app_biz_delivery_price")]
	public class DeliveryPrice
	{
		public DeliveryPrice()
		{
		}
		
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }

		[Index("src",Order =1)]
		[Index("dst", Order = 2)]
		public int SrcLocationId { get; set; }
		[Index("src", Order = 2)]
		[Index("dst", Order = 1)]
		public int DstLocationId { get; set; }

		[ForeignKey(nameof(SrcLocationId))]
		public DeliveryLocation SrcLocation { get; set; }

		[ForeignKey(nameof(DstLocationId))]
		public DeliveryLocation DstLocation { get; set; }

		public decimal FirstPrice { get; set; }
		public int FirstWeight { get; set; }
		public decimal AdditionalPrice { get; set; }
		public int AdditionalWeight { get; set; }
	}
}
