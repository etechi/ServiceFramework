using SF.Sys.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Biz.Delivery.DataModels
{

    /// <summary>
    /// 运费
    /// </summary>
    public class DataDeliveryPrice
	{
		public DataDeliveryPrice()
		{
		}
		
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long Id { get; set; }

		[Index("src",Order =1)]
		[Index("dst", Order = 2)]
		public long SrcLocationId { get; set; }

		[Index("src", Order = 2)]
		[Index("dst", Order = 1)]
		public long DstLocationId { get; set; }


		public decimal FirstPrice { get; set; }
		public decimal FirstWeight { get; set; }
		public decimal AdditionalPrice { get; set; }
		public decimal AdditionalWeight { get; set; }
	}
}
