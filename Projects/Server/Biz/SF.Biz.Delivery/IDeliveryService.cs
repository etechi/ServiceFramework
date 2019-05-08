using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Biz.Delivery
{
    public class DeliveryQueryArguments:QueryArgument
	{
        /// <summary>
        /// 收件人
        /// </summary>
		[EntityIdent(typeof(User))]
		[Ignore]
		public long ReceiverId { get; set; }

        /// <summary>
        /// 发货状态
        /// </summary>
        public DeliveryState? State { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateQueryRange CreateTime { get; set; }
        
        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(100)]
		public string Search { get; set; }
	}
    public interface IDeliveryService
	{
		Task<DeliveryInternal> Get(int Id);
		Task<QueryResult<DeliveryInternal>> Query(DeliveryQueryArguments args);
	}


	public class VIADDeliveryItem
	{
		public long Id { get; set; }
		public long VIADSpecId { get; set; }
		public string PayloadId { get; set; }
		public string PayloadSpecId { get; set; }
		public string Title { get; set; }
	}
	public class VIADDelivery
	{
		public long Id { get; set; }
		public long UserId { get; set; }
		public IEnumerable<VIADDeliveryItem> Items { get; set; }
	}
	public interface IVIADDeliverySupport
	{
		Task<VIADDelivery[]> Query(int Limit);
		Task AutoDeliveryCompleted(long DeliveryId,Dictionary<long, Tuple<long, string>> Records,DateTime Time);
		Task SendEvent(long DeliveryId);
	}
}
