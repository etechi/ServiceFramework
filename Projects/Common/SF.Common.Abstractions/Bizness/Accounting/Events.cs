using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SF.Bizness.Accounting
{
	public class AccountDepositCompletedEvent : ServiceProtocol.Events.IEvent
    {
        public string Source => "账户";
        public string Type => "充值完成";
        public int Ident { get; }//AccountDepositRecord-xxx
        public string Desc { get; }
        public decimal Amount { get; }
        public int UserId { get; }
        public string AccountTitle { get; }
        public AccountDepositCompletedEvent(int id, int UserId, string Desc, decimal Amount, string AccountTitle)
        {
            this.Ident = id;
            this.UserId = UserId;
            this.Desc = Desc;
            this.Amount = Amount;
            this.AccountTitle = AccountTitle;
        }
    }
    public class AccountRefundCompletedEvent : ServiceProtocol.Events.IEvent
    {
        public string Source => "账户";
        public string Type => "退款完成";
        public int Ident { get; }//AccountDepositRecord-xxx
        public string Desc { get; }
        public decimal Amount { get; }
        public int UserId { get; }
        public bool Success { get; }
        public AccountRefundCompletedEvent(int id, int UserId, string Desc, decimal Amount, bool Success)
        {
            this.Ident = id;
            this.UserId = UserId;
            this.Desc = Desc;
            this.Amount = Amount;
            this.Success = Success;
        }
    }
}
