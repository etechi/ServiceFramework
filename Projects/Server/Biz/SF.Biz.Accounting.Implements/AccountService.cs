using SF.Sys.Data;
using SF.Sys.TimeServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{

    public class AccountService:IAccountService
	{	
		public IDataScope DataScope { get; }
		public ITimeService TimeService { get; }
		
		public AccountService(
			IDataScope DataScope, 
			ITimeService TimeService
            )
		{
			this.DataScope = DataScope;
			this.TimeService = TimeService;
			
        }


        public async Task<long> GetTitleId(string Title)
        {
            if (Title == null)
                Title = "balance";
            var re=await DataScope.Use("查询科目ID", ctx => 
                ctx.Queryable<DataModels.DataAccountTitle>()
                .Where(t => t.Ident == Title)
                .Select(t => t.Id)
                .SingleOrDefaultAsync()
                );
            return re;
        }

        public async Task<Account> GetAccount(long CaptionId, long OwnerId)
        {
            var re = await DataScope.Use("查询科目", ctx =>
                 ctx.Queryable<DataModels.DataAccount>()
                .Where(a => a.AccountTitleId == CaptionId && a.OwnerId.Equals(OwnerId))
                .Select(a => new Account
                {
                    Inbound = a.Inbound,
                    Outbound = a.Outbound

                })
                .SingleOrDefaultAsync()
            );
            return re ?? new Account();
        }
        public async Task<decimal> GetTitleValue(long OwnerId, string Title)
		{
            if (string.IsNullOrEmpty(Title))
                Title = "balance";
            var re = await DataScope.Use("查询科目", ctx =>
                (from a in ctx.Queryable<DataModels.DataAccount>()
                 join t in ctx.Queryable<DataModels.DataAccountTitle>() on a.AccountTitleId equals t.Id
                 where t.Ident == Title && a.OwnerId.Equals(OwnerId)
                 select a.Inbound - a.Outbound
                ).SingleOrDefaultAsync()
                );

            return re;
		}
        public async Task<decimal> GetSettlementBalance(long OwnerId)
        {
            var re = await DataScope.Use("查询可结算余额", ctx =>
               (from a in ctx.Queryable<DataModels.DataAccount>()
                join t in ctx.Queryable<DataModels.DataAccountTitle>() on a.AccountTitleId equals t.Id
                where a.OwnerId == OwnerId && t.SettlementEnabled
                select a.Inbound - a.Outbound).DefaultIfEmpty(0).SumAsync()
                    );
            return re;;
        }
        public async Task<Dictionary<long,decimal>> GetSettlementBalances(long[] OwnerIds)
        {
            var re = await DataScope.Use("查询可结算余额", ctx =>
                (from a in ctx.Queryable<DataModels.DataAccount>()
                 join t in ctx.Queryable<DataModels.DataAccountTitle>() on a.AccountTitleId equals t.Id
                 where OwnerIds.Contains(a.OwnerId.Value) && t.SettlementEnabled
                 group a by a.OwnerId.Value into g
                 select new { id = g.Key, balance = g.Select(i => i.Inbound - i.Outbound).DefaultIfEmpty().Sum() }
                ).ToDictionaryAsync(i => i.id, i => i.balance)
                );
            return re;
        }



    }
}
