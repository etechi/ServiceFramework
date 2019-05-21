using SF.Sys.Data;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{

    public class AccountUpdater
	{	
		public IDataContext Context { get; }
        public Dictionary<(long TitleId,long UserId),DataModels.DataAccount> Accounts { get; }
        public IIdentGenerator IdentGenerator { get; }
        public AccountUpdater(IDataContext Context,IIdentGenerator IdentGenerator)
        {
            this.Context = Context;
            this.IdentGenerator = IdentGenerator;
            this.Accounts = new Dictionary<(long TitleId, long UserId), DataModels.DataAccount>();
        }
        public async Task LoadAccounts(params (long TitleId,long UserId)[] Ids)
        {


            foreach (var grp in Ids.GroupBy(i=>i.TitleId,i=>i.UserId))
            {
                var cid = grp.Key;
                var oids = new HashSet<long>(grp.Where(i => i != 0 && !Accounts.ContainsKey((cid, i))));

                foreach (var a in Context.Set<DataModels.DataAccount>().CachedEntities().Where(
                     a => a.AccountTitleId == grp.Key && oids.Contains(a.OwnerId.Value)
                     ))
                {
                    Accounts.Add((cid, a.OwnerId.Value), a);
                    oids.Remove(a.OwnerId.Value);
                }

                var re = await Context.Queryable<DataModels.DataAccount>()
                    .Where(a => a.AccountTitleId == cid && oids.Contains(a.OwnerId.Value))
                    .ToArrayAsync();

                foreach (var a in re)
                    Accounts.Add((cid, a.OwnerId.Value), a);
            }

        }

        public async Task<decimal> Update(
            long TitleId,
            long OwnerId,
            decimal InboundDiff,
            decimal OutboundDiff,
            DateTime Time
            )
        {
            //系统账号
            if (OwnerId == 0)
                return 0;

            DataModels.DataAccount acc;
            if (!Accounts.TryGetValue((TitleId, OwnerId), out acc))
            {
                acc = new DataModels.DataAccount
                {
                    Id = await IdentGenerator.GenerateAsync<DataModels.DataAccount>(),
                    AccountTitleId = TitleId,
                    OwnerId = OwnerId
                };
                Context.Add(acc);
                Accounts.Add((TitleId, OwnerId), acc);
            }
            else
                Context.Update(acc);

            var newInbound = acc.Inbound + InboundDiff;
            var newOutbound = acc.Outbound + OutboundDiff;
            if (newOutbound > newInbound)
                throw new BalanceNotEnough(acc.Inbound - acc.Outbound, OutboundDiff - InboundDiff);

            acc.Inbound = newInbound;
            acc.Outbound = newOutbound;
            acc.CurValue = newInbound - newOutbound;
            acc.UpdatedTime = Time;
            return acc.CurValue;
        }
    }
}
