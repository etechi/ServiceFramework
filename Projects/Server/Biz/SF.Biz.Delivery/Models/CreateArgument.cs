using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Delivery
{
	
	public class DeliveryCreateArgument
	{
		public string Title { get; set; }
		public long SenderId { get; set; }
		public long ReceiverId { get; set; }

		public long? SourceAddressId { get; set; }
		public long? DestAddressId { get; set; }
		public string TrackEntityIdent{ get; set; }
		public long TotalQuantity { get; set; }
		public long Weight { get; set; }
		public DeliveryItemCreateArgument[] Items { get; set; }
	}
	public class DeliveryItemCreateArgument
	{
		public string Title { get; set; }
		public string PayloadEntityIdent { get; set; }
        public string PayloadSpecEntityIdent { get; set; }
        public int Quantity { get; set; }
        public bool VirtualItem { get; set; }
		public long? VIADSpecId { get; set; }
	}
    public class DeliveryCreateResult
    {
        public long DeliveryId { get; set; }
        public long[] DeliveryItemIds { get; set; }
    }
}
