﻿using SF.Sys.Clients;
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
		public long RemindId { get; set; }
		public string ClientType { get; set; }
		public string HttpRedirest { get; set; }
        public string OpAddress { get; set; }
        public ClientDeviceType OpDevice { get; set; }
	}
   
    public class DepositStartResult
	{
        public long Id { get; set; }
		public IDictionary<string,string> PaymentStartResult { get; set; }
	}

    public interface IDepositService
	{
        Task<long> CreateDeposit(DepositArgument Arg);
        Task<DepositStartResult> StartDeposit(long Id, Biz.Payments.StartRequestInfo RequestInfo);
        
        
        Task<DepositRecord> GetDepositResult(long Id,bool Query=false,bool Remind=false);

    }
}
