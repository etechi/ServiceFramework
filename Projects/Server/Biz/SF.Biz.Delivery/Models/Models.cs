using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Delivery
{
    public class DeliveryEvent
    {
        public string Title { get; }
        public int DeliveryId { get; }
        public string UserId { get; }
        public string ExpressCode { get; }
        public string ExpressName { get; }
        public bool HasVirtualItem { get; }
        public bool HasObjectItem { get;  }
        public DeliveryEvent(
            int DeliveryId, 
            string UserId, 
            string Title,
            string ExpressName, 
            string ExpressCode,
            bool HasObjectItem,
            bool HasVirtualItem
            )
        {
            this.DeliveryId = DeliveryId;
            this.UserId = UserId;
            this.ExpressName = ExpressName;
            this.ExpressCode = ExpressCode;
            this.Title = Title;
            this.HasVirtualItem = HasVirtualItem;
            this.HasObjectItem = HasObjectItem;
        }
    }
    public enum DeliveryState
	{
        /// <summary>
        /// 等待发货
        /// </summary>
        DeliveryWaiting = 0,
        /// <summary>
        /// 待录单号
        /// </summary>
		CodeWaiting = 10,
        /// <summary>
        /// 已发货
        /// </summary>
        Delivering = 20,
        /// <summary>
        /// 已签收
        /// </summary>
        Received = 30
	}
	public class DeliveryStatus
	{
		public int Id { get; set; }
		public DeliveryState State { get; set; }
		public string Location { get; set; }
		public string Address { get; set; }
		public string ContactName { get; set; }
		public string ContactPhone { get; set; }

		public string TransportName { get; set; }
		public string TransportCode { get; set; }
		public DateTime StateTime { get; set; }
        public bool HasObjectItem { get; set; }
        public bool HasVirtualItem { get; set; }
    }
	
}
