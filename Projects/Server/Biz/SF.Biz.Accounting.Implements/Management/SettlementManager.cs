#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0


using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Biz.Accounting.DataModels;
using SF.Biz.Payments;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Logging;
using SF.Sys.Services;

namespace SF.Biz.Accounting
{

    public class SettlementManager :
        AutoQueryableEntitySource<ObjectKey<long>, SettlementRecordDetail, SettlementRecord, SettlementRecordQueryArguments, DataModels.DataSettlementRecord>,
        ISettlementManager
    {
        Lazy<ICollectService> CollectService { get; }
        Lazy<ITransferService> TransferService { get; }
        public SettlementManager(
            IEntityServiceContext ServiceContext, 
            Lazy<ICollectService> CollectService,
            Lazy<ITransferService> TransferService
            ) : base(ServiceContext)
        {
            this.CollectService = CollectService;
            this.TransferService = TransferService;
        }

        public Task<SettlementStatus> Start(SettlementStartArgument Argument)
        {
            Ensure.Assert(Argument.TotalAmount>0, "结算金额必须大于0");
            Ensure.Assert(Argument.Items.All(i => i.Amount > 0), "单项金额必须大于0");
            Ensure.Equal(Argument.TotalAmount, Argument.Items.Select(i => i.Amount).Sum(), "金额不一致");
            Ensure.Equal(Argument.Items.Length, Argument.Items.Select(i => i.AccountTitle).Distinct().Count(), "结算项目科目有重复");
            decimal CollectAmount = 0;
            if (Argument.CollectPaymentPlatformId.HasValue)
            {
                Ensure.HasContent(Argument.CollectHttpRedirect, "收款重定向页面");
                Ensure.HasContent(Argument.CollectTitle, "收款科目");
                Ensure.Equal(1, Argument.Items.Count(i => i.AccountTitle == Argument.CollectTitle), "找不到收款科目对应的项目");
                CollectAmount = Argument.Items.First(i => i.AccountTitle == Argument.CollectTitle).Amount;
            }


            return DataScope.Use("创建结算记录", async ctx =>
            {

                var titles = await ctx.Queryable<DataModels.DataAccountTitle>()
                               .Where(t=>t.LogicState==EntityLogicState.Enabled)
                               .ToDictionaryAsync(t => t.Ident, t => t.Id);

                var now = Now;
                var rid = await IdentGenerator.GenerateAsync<DataModels.DataSettlementRecord>();
                var iids = await IdentGenerator.GenerateAsync<DataSettlementItemRecord>(Argument.Items.Length);
                var record = ctx.Add(new DataModels.DataSettlementRecord
                {
                    Id = rid,
                    TotalAmount = Argument.TotalAmount,
                    LeftAmount=Argument.TotalAmount,
                    BizParent = Argument.BizParent,
                    BizRoot = Argument.BizRoot,
                    BuyerId = Argument.BuyerId,
                    SellerId = Argument.SellerId,
                    CreatedTime = now,
                    DstTitleId = titles[Argument.DstTitle],
                    Name=Argument.Name,
                    OpAddress=Argument.ClientInfo.ClientAddress,
                    OpDevice=Argument.ClientInfo.DeviceType,
                    OwnerId=Argument.ClientInfo.OperatorId,
                    PrepayTitleId =titles[Argument.PrepayTitle],
                    UpdatedTime=now,
                    UpdatorId=Argument.BuyerId,
                    CollectPaymentPlatformId = Argument.CollectPaymentPlatformId,
                    CollectTitleId= Argument.CollectTitle.HasContent()?(long?)titles[Argument.CollectTitle]:null,
                    CollectAmount= CollectAmount,

                    Items = Argument.Items.Select((i,idx)=>new DataSettlementItemRecord
                    {
                        Id= iids[idx],
                        SettlementRecordId=rid,
                        Index=idx,
                        LeftAmount=i.Amount,
                        TotalAmount=i.Amount,
                        TitleId=titles[i.AccountTitle]
                    } ).ToArray()
                    
                });

                var result=new SettlementStatus
                {
                    Id=record.Id
                };
                if (!record.CollectPaymentPlatformId.HasValue)
                {
                    await Transform(record, Argument.ClientInfo);
                    record.State = SettlementState.WaitConfirm;
                    result.State = record.State;
                }
                else {
                    var amount = record.Items.Single(i => i.TitleId == record.CollectTitleId.Value).TotalAmount;
                    var re=await CollectService.Value.Start(new CollectRequest
                    {
                        Amount = amount,
                        BizParent = new TrackIdent("结算", "SettlementRecord", record.Id, Now.ToString("yyyyMMddHHmmss")),
                        BizRoot = record.BizRoot,
                        ClientInfo = Argument.ClientInfo,
                        Desc = Argument.Desc,
                        PaymentPlatformId = record.CollectPaymentPlatformId.Value,
                        StartTime = Now,
                        Title = record.Name,
                        HttpRedirect = Argument.CollectHttpRedirect,
                    });
                    record.CollectRecordId = re.Id;
                    result.Expires = re.Expires;
                    result.CollectStartResult = re.Data;
                    result.State=record.State = SettlementState.Processing;
                }

                await ctx.SaveChangesAsync();

                return result;
            });
        }

        async Task Transform(DataSettlementRecord record,ClientInfo ClientInfo)
        {
            var Ids=await TransferService.Value.Transfer(new TransferArgument
            {
                ClientInfo=ClientInfo,
                Items=record.Items.Select(i=>new TransferArgumentItem
                {
                    Amount=i.TotalAmount,
                    BizParent= new TrackIdent("结算", "SettlementItemRecord", i.Id),
                    BizRecordIndex=i.Index,
                    BizRoot=record.BizRoot,
                    Description=record.Desc,
                    DstId=record.SellerId,
                    DstTitleId=record.PrepayTitleId,
                    SrcId=record.BuyerId,
                    SrcTitleId=i.TitleId
                }).ToArray()
            });

            foreach (var pair in record.Items.Zip(Ids, (it, i) => (it, i)))
                pair.it.TransferRecordId = pair.i;
        }
        public Task Cancel(long Id, ClientInfo ClientInfo)
        {
            if (ClientInfo == null)
                ClientInfo = ServiceContext.ClientService.GetClientInfo();

            return DataScope.Use("取消结算", async ctx =>
            {
                var record = await ctx.Queryable<DataModels.DataSettlementRecord>(false)
                    .Where(r => r.Id == Id && r.LogicState == EntityLogicState.Enabled)
                    .Include(r => r.Items)
                    .SingleOrDefaultAsync();
                if (record == null)
                    throw new PublicArgumentException("找不到结算记录:" + Id);

                if (record.State != SettlementState.Processing)
                    throw new PublicInvalidOperationException("收款已结束，不能取消:" + record.Id);

                if(record.CollectRecordId.HasValue)
                    await CollectService.Value.Cancel(record.CollectRecordId.Value, ClientInfo);
                record.State = SettlementState.Cancelled;
                record.UpdatedTime = Now;
                record.UpdatorId = ClientInfo.OperatorId;
                ctx.Update(record);
                await ctx.SaveChangesAsync();
               
            });
        }
        public Task<SettlementStatus> UpdateAndQueryStatus(long Id,ClientInfo ClientInfo)
        {
            if(ClientInfo==null)
                ClientInfo = ServiceContext.ClientService.GetClientInfo();

            return DataScope.Use("更新结算记录", async ctx =>
            
            {
                var record = await ctx.Queryable<DataModels.DataSettlementRecord>(false)
                    .Where(r=>r.Id==Id && r.LogicState==EntityLogicState.Enabled)
                    .Include(r=>r.Items)
                    .SingleOrDefaultAsync();
                if (record == null)
                    throw new PublicArgumentException("找不到结算记录:" + Id);


                if (record.State != SettlementState.Processing)
                    return new SettlementStatus
                    {
                        Id = Id,
                        State = record.State
                    };

                if (record.CollectRecordId.HasValue)
                {
                    var re=await CollectService.Value.GetResult(record.CollectRecordId.Value, true);
                    if (!re.Response.CompletedTime.HasValue)
                        return new SettlementStatus
                        {
                            Id = Id,
                            State = SettlementState.Processing
                        };

                    if (re.Response.AmountCollected != record.CollectAmount && re.Response.Error.IsNullOrEmpty())
                    {
                        re.Response.Error = $"收款金额不符";
                        Logger.Error($"收款金额{re.Response.AmountCollected}和所需金额{record.CollectAmount}不符,结算记录:{record.Id}");
                    }
                    if (re.Response.Error.HasContent())
                    {
                        record.State = SettlementState.Failed;
                        record.Error = re.Response.Error;
                    }
                    else
                    {
                        var updater = new AccountUpdater(ctx,IdentGenerator);
                        await updater.LoadAccounts((record.CollectTitleId.Value, record.BuyerId));
                        await updater.Update(record.CollectTitleId.Value, record.BuyerId, record.CollectAmount, 0, Now);

                        record.State = SettlementState.WaitConfirm;
                    }
                }
                else
                    record.State = SettlementState.WaitConfirm;

                record.UpdatedTime = Now;
                record.UpdatorId = ClientInfo.OperatorId;

                if(record.State==SettlementState.WaitConfirm)
                    await Transform(record, ClientInfo);

                ctx.Update(record);
                await ctx.SaveChangesAsync();
                return new SettlementStatus
                {
                    Id = Id,
                    State = SettlementState.WaitConfirm
                };
            });
        }

        public Task<SettlementStatus> Confirm(long Id, ClientInfo ClientInfo = null)
        {
            if (ClientInfo == null)
                ClientInfo = ServiceContext.ClientService.GetClientInfo();

            return DataScope.Use("确认结算记录", async ctx =>
            {
                var record = await ctx.Queryable<DataSettlementRecord>(false)
                    .Where(r => r.Id == Id && r.LogicState == EntityLogicState.Enabled)
                    .SingleOrDefaultAsync();
                if (record == null)
                    throw new PublicArgumentException("找不到结算记录:" + Id);


                if (record.State != SettlementState.WaitConfirm)
                    throw new PublicInvalidOperationException("结算未就绪，不能确认:" + record.Id);

                var tids=await TransferService.Value.Transfer(new TransferArgument
                {
                    ClientInfo = ClientInfo,
                    Items = new[] { new TransferArgumentItem
                    {
                        Amount = record.LeftAmount,
                        BizParent = new TrackIdent("结算", "SettlementRecord", record.Id),
                        BizRoot = record.BizRoot,
                        BizRecordIndex = 0,
                        Description = record.Desc,
                        DstId = record.SellerId,
                        DstTitleId = record.DstTitleId,
                        SrcId = record.SellerId,
                        SrcTitleId = record.PrepayTitleId
                    } }
                });

                record.LastTransferRecordId = tids[0];
                record.State = SettlementState.Success;
                record.UpdatorId = ClientInfo.OperatorId;
                record.UpdatedTime = Now;
                ctx.Update(record);
                await ctx.SaveChangesAsync();
                return new SettlementStatus
                {
                    Id = record.Id,
                    Message = record.Error,
                    State = record.State
                };
            });
        }
    }

}
