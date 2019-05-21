using SF.Sys.Clients;
using SF.Sys.Entities;
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
        public string HttpRedirest { get; set; }

		public ClientInfo ClientInfo { get; set; }
	}
   
    public class DepositStartResult
	{
        public long Id { get; set; }
        public DateTime Expires { get; set; }
		public IReadOnlyDictionary<string,string> PaymentArguments { get; set; }
	}

    public interface IDepositService
	{
        Task<DepositStartResult> Start(DepositArgument Arg);
        
        
        Task<DepositRecord> GetResult(long Id,bool Query=false,bool Remind=false);

    }
}
