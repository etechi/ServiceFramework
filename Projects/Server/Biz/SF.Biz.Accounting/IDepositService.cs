using SF.Sys.Clients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{
    public class DepositArgument
	{
		public string AccountTitle { get; set; }
		public long DstId { get; set; }
		public long PaymentPlatformId { get; set; }
		public long OperatorId { get; set; }
		public decimal Amount { get; set; }
		public string Description { get; set; }
		public string TrackEntityIdent { get; set; }
		public string CallbackName { get; set; }
		public string CallbackContext { get; set; }
		public string ClientType { get; set; }
		public string HttpRedirest { get; set; }
        public string OpAddress { get; set; }
        public ClientDeviceType OpDevice { get; set; }
	}
   
    public class DepositStartResult
	{
        public int Id { get; set; }
		public string RecordId { get; set; }
		public IDictionary<string,string> PaymentStartResult { get; set; }
	}

    public interface IDepositService
	{
        Task<int> CreateDeposit(DepositArgument Arg);
        Task<DepositStartResult> StartDeposit(int Id, Biz.Payments.StartRequestInfo RequestInfo);
        Task CompleteDeposit(int recordId, string lowerRecordId, Exception exception);
        
        Task<DepositRecord> RefreshDepositRecord(int Id, int DstId);

    }
}
