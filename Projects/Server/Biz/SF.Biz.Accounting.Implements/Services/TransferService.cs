using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Entities;
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


     
        //public async Task<long[]> Settlement(SettlementArgument Arg)
        //{
        //    return await DataScope.Use("结算操作", async ctx =>
        //    {
        //        var accs = await (from a in ctx.Queryable<DataModels.DataAccount>()
        //                          join t in ctx.Queryable<DataModels.DataAccountTitle>() on a.AccountTitleId equals t.Id
        //                          where a.OwnerId == Arg.SrcId && (t.Ident == Arg.FirstTitle || t.SettlementEnabled) && a.Inbound > a.Outbound
        //                          orderby t.SettlementOrder
        //                          select new { amount = a.Inbound - a.Outbound, title = t.Ident }
        //            ).ToArrayAsync();
        //        if (accs.Length == 0)
        //            throw new BalanceNotEnough(0, Arg.Amount);
        //        var re = accs.Aggregate(
        //            new { idx = 0, amount = Arg.Amount, items = new List<TransferArgumentItem>() },
        //            (c, acc) =>
        //            {
        //                if (c.amount == 0)
        //                    return c;
        //                var item = new TransferArgumentItem
        //                {
        //                    Amount = Math.Min(c.amount, acc.amount),
        //                    BizRoot=Arg.BizRoot,
        //                    BizParent=Arg.BizParent,

        //                    BizRecordIndex = c.idx,
        //                    Description = Arg.Description,
        //                    DstId = Arg.DstId,
        //                    DstTitle = Arg.DstTitle,
        //                    SrcId = Arg.SrcId,
        //                    SrcTitle = acc.title,
        //                };
        //                c.items.Add(item);
        //                return new { idx = c.idx + 1, amount = c.amount - item.Amount, items = c.items };
        //            });
        //        if (re.amount > 0)
        //            throw new BalanceNotEnough(Arg.Amount - re.amount, Arg.Amount);

        //        return await Transfer(ctx,new TransferArgument
        //        {
        //            ClientInfo=new Sys.Clients.ClientInfo{
        //                OperatorId= Arg.OperatorId,
        //                OpAddress = Arg.OpAddress,
        //                OpDevice = Arg.OpDevice
        //            Items = re.items.ToArray(),
                    
        //        });
        //    });
        //}
        async Task<long[]> Transfer(IDataContext ctx,TransferArgument Arg)
        {
            
            var titles =Arg.Items.All(i=>i.DstTitleId.HasValue && i.SrcTitleId.HasValue)?null:
                await ctx.Queryable<DataModels.DataAccountTitle>()
                .Where(t => t.LogicState == EntityLogicState.Enabled)
                .Select(t => new { key = t.Ident, value = t.Id })
                .ToDictionaryAsync(t => t.key, t => t.value);


            var updater = new AccountUpdater(ctx,IdentGenerator.Value);
            await updater.LoadAccounts(Arg.Items.SelectMany(i => new[] {
                         (title:i.DstTitleId ?? titles[i.DstTitle],dst:i.DstId),
                         (title:i.SrcTitleId ?? titles[i.SrcTitle],dst:i.SrcId)
                     }).ToArray());

            var time = TimeService.Now;
            var trs = new List<DataModels.DataTransferRecord>();
            var ids = await IdentGenerator.Value.GenerateAsync<DataModels.DataTransferRecord>(Arg.Items.Length);
            var index = 0;
            foreach (var item in Arg.Items)
            {
                var srcCurValue = await updater.Update(item.SrcTitleId?? titles[item.SrcTitle], item.SrcId, 0, item.Amount, time);
                var dstCurValue = await updater.Update(item.DstTitleId?? titles[item.DstTitle], item.DstId, item.Amount, 0, time);
                trs.Add(ctx.Add(new DataModels.DataTransferRecord
                {
                    Id = ids[index++],
                    SrcTitleId =item.SrcTitleId?? titles[item.SrcTitle],
                    DstTitleId =item.DstTitleId?? titles[item.DstTitle],
                    SrcId = item.SrcId,
                    DstId = item.DstId,
                    Time = time,
                    Amount = item.Amount,
                    SrcCurValue = srcCurValue,
                    DstCurValue = dstCurValue,
                    Title = item.Description.Limit(200),
                    BizParent=item.BizParent,
                    BizRoot=item.BizRoot,
                    BizRecordIndex = item.BizRecordIndex
                }));
            }
            //var rec = BuildTransferRecord(titles,Arg.OperatorId, time, Arg.TopItem);
            await ctx.SaveChangesAsync();
            return ids;
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
