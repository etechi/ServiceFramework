using SF.Sys.Clients;
using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{
    public class ClientDepositRecordQueryArguments:QueryArgument
    {
        public string Ident { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? PaymentPlatformId { get; set; }
        public DepositState? State { get; set; }
    }
    public class ClientDepositArguments
    {
        public decimal Amount { get; set; }
        public long PaymentPlatformId { get; set; }
        public string Redirect { get; set; }
    }
    public interface IAccountingService
    {
        Task<DepositRecord> GetDepositRecord(long id);
        Task<QueryResult<DepositRecord>> QueryDepositRecords(ClientDepositRecordQueryArguments Args);
        Task<decimal> Balance();
        Task<DepositRecord> RefreshDepositRecord(long Id);
        Task<DepositStartResult> Deposit(ClientDepositArguments Args);
    }
}
