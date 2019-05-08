using SF.Sys.Clients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{
    public interface IAccountService
	{
        Task<long> GetTitleId(string Title);
        Task<decimal> GetTitleValue(long OwnerId, string Title);
        Task<Account> GetAccount(long TitleId, long OwnerId);
        Task<decimal> GetSettlementBalance(long OwnerId);
        Task<Dictionary<long, decimal>> GetSettlementBalances(long[] OwnerIds);

    }
}
