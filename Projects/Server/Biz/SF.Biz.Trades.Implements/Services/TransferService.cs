using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Events;
using SF.Sys.Logging;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{
    /// <summary>
    /// 账户充值记录
    /// </summary>
    public class TransferService:
		ITransferService
	{
        public IDataScope DataScope { get; }
        public ITimeService TimeService { get; }
        public Lazy<IIdentGenerator<DataModels.DataTransferRecord>> IdentGenerator { get; }
        public ILogger Logger { get; }
        public Lazy<IEventEmitService> EventEmitService { get; }
        public TransferService(
            IDataScope DataScope,
            ITimeService TimeService,
            Lazy<IIdentGenerator<DataModels.DataTransferRecord>> IdentGenerator,
            ILogger<DepositService> Logger,
            Lazy<IEventEmitService> EventEmitService
            )
        {
            this.DataScope = DataScope;
            this.TimeService = TimeService;
            this.IdentGenerator = IdentGenerator;
            this.Logger = Logger;
            this.EventEmitService = EventEmitService;
        }


        decimal UpdateAccount(
            IDataContext ctx,
            Dictionary<KeyValuePair<long, long>, DataModels.DataAccount> accounts,
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
            var key = new KeyValuePair<long, long>(TitleId, OwnerId);
            if (!accounts.TryGetValue(key, out acc))
            {
                acc = new DataModels.DataAccount
                {
                    AccountTitleId = TitleId,
                    OwnerId = OwnerId
                };
                ctx.Add(acc);
                accounts.Add(key, acc);
            }
            else
                ctx.Update(acc);

            var newInbound = acc.Inbound + InboundDiff;
            var newOutbound = acc.Outbound + OutboundDiff;
            if (newOutbound > newInbound)
                throw new BalanceNotEnough(acc.Inbound - acc.Outbound, OutboundDiff- InboundDiff);

            acc.Inbound = newInbound;
            acc.Outbound = newOutbound;
            acc.CurValue = newInbound - newOutbound;
            acc.UpdatedTime = Time;
            return acc.CurValue;
        }
        public async Task<long[]> Settlement(SettlementArgument Arg)
        {
            return await DataScope.Use("结算操作", async ctx =>
            {
                var accs = await (from a in ctx.Queryable<DataModels.DataAccount>()
                                  join t in ctx.Queryable<DataModels.DataAccountTitle>() on a.AccountTitleId equals t.Id
                                  where a.OwnerId == Arg.SrcId && (t.Ident == Arg.FirstTitle || t.SettlementEnabled) && a.Inbound > a.Outbound
                                  orderby t.SettlementOrder
                                  select new { amount = a.Inbound - a.Outbound, title = t.Ident }
                    ).ToArrayAsync();
                if (accs.Length == 0)
                    throw new BalanceNotEnough(0, Arg.Amount);
                var re = accs.Aggregate(
                    new { idx = 0, amount = Arg.Amount, items = new List<TransferArgumentItem>() },
                    (c, acc) =>
                    {
                        if (c.amount == 0)
                            return c;
                        var item = new TransferArgumentItem
                        {
                            Amount = Math.Min(c.amount, acc.amount),
                            TrackEntityIdent = Arg.TraceEntityIdent,

                            BizRecordIndex = c.idx,
                            Description = Arg.Description,
                            DstId = Arg.DstId,
                            DstTitle = Arg.DstTitle,
                            SrcId = Arg.SrcId,
                            SrcTitle = acc.title,
                        };
                        c.items.Add(item);
                        return new { idx = c.idx + 1, amount = c.amount - item.Amount, items = c.items };
                    });
                if (re.amount > 0)
                    throw new BalanceNotEnough(Arg.Amount - re.amount, Arg.Amount);
                return await Transfer(ctx,new TransferArgument
                {
                    OperatorId = Arg.OperatorId,
                    Items = re.items.ToArray(),
                    OpAddress = Arg.OpAddress,
                    OpDevice = Arg.OpDevice
                });
            });
        }
        async Task<long[]> Transfer(IDataContext ctx,TransferArgument Arg)
        {
            var titles = await ctx.Queryable<DataModels.DataAccountTitle>()
                .Select(t => new { key = t.Ident, value = t.Id })
                .ToDictionaryAsync(t => t.key, t => t.value);

            var acc_groups = Arg.Items.SelectMany(i => new[] {
                         new KeyValuePair<long,long>(titles[i.DstTitle],i.DstId),
                         new KeyValuePair<long,long>(titles[i.SrcTitle],i.SrcId)
                     }).GroupBy(p => p.Key, p => p.Value);

            var accs = new Dictionary<KeyValuePair<long, long>, DataModels.DataAccount>();
            foreach (var grp in acc_groups)
            {
                var cid = grp.Key;
                var oids = grp.Where(id => id != 0).ToArray();
                var re = await ctx.Queryable<DataModels.DataAccount>()
                    .Where(a => a.AccountTitleId == cid && oids.Contains(a.OwnerId.Value))
                    .ToArrayAsync();
                foreach (var a in re)
                    accs.Add(new KeyValuePair<long, long>(cid, a.OwnerId.Value), a);
            }

            var time = TimeService.Now;
            var trs = new List<DataModels.DataTransferRecord>();
            foreach (var item in Arg.Items)
            {
                var srcCurValue = UpdateAccount(ctx, accs, titles[item.SrcTitle], item.SrcId, 0, item.Amount, time);
                var dstCurValue = UpdateAccount(ctx, accs, titles[item.DstTitle], item.DstId, item.Amount, 0, time);
                trs.Add(ctx.Add(new DataModels.DataTransferRecord
                {
                    SrcTitleId = titles[item.SrcTitle],
                    DstTitleId = titles[item.DstTitle],
                    SrcId = item.SrcId,
                    DstId = item.DstId,
                    Time = time,
                    Amount = item.Amount,
                    SrcCurValue = srcCurValue,
                    DstCurValue = dstCurValue,
                    Title = item.Description.Limit(200),
                    TrackEntityIdent = item.TrackEntityIdent,
                    BizRecordIndex = item.BizRecordIndex
                }));
            }
            //var rec = BuildTransferRecord(titles,Arg.OperatorId, time, Arg.TopItem);
            await ctx.SaveChangesAsync();
            return trs.Select(t => t.Id).ToArray();
        }


        public async Task<long[]> Transfer(TransferArgument Arg)
        {
            return await DataScope.Use("转账操作", async ctx =>
            {
                return await Transfer(ctx, Arg);
            });
        }



    }
}
