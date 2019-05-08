using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{
    public class TransferArgumentItem
	{
		public string SrcTitle { get; set; }
		public string DstTitle { get; set; }
		public long SrcId { get; set; }
		public long DstId { get; set; }
		public decimal Amount { get; set; }
		public string Description { get; set; }
        public string TrackEntityIdent { get; set; }
        public int BizRecordIndex { get; set; }
        //public TransferArgumentItem<TUserKey>[] Children { get; set; }
    }
	public class TransferArgument
	{
		public TransferArgumentItem[] Items { get; set; }
		public long OperatorId { get; set; }
        public string OpAddress { get; set; }
        public ClientDeviceType OpDevice { get; set; }
    }
    public class SettlementArgument
    {
        public string FirstTitle { get; set; }
        public long SrcId { get; set; }
        public string DstTitle { get; set; }
        public long DstId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string TraceEntityIdent { get; set; }
        public long OperatorId { get; set; }
        public string OpAddress { get; set; }
        public ClientDeviceType OpDevice { get; set; }
    }

    public interface ITransferService
    {
        Task<long[]> Settlement(SettlementArgument Arg);
        Task<long[]> Transfer(TransferArgument Arg);
    }
}
