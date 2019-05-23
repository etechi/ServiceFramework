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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Biz.Accounting.DataModels;
using SF.Biz.Payments;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Data;
using SF.Sys.Entities;

namespace SF.Biz.Accounting
{

    public class SettlementRollbackManager :
        AutoQueryableEntitySource<ObjectKey<long>, SettlementRollbackRecordDetail, SettlementRollbackRecord, SettlementRollbackRecordQueryArguments, DataModels.DataSettlementRollbackRecord>,
        ISettlementRollbackManager
    {
        Lazy<IRefundService> RefundService { get; }
        Lazy<ITransferService> TransferService { get; }
        public SettlementRollbackManager(
            IEntityServiceContext ServiceContext, 
            Lazy<IRefundService> RefundService,
            Lazy<ITransferService> TransferService
            ) : base(ServiceContext)
        {
            this.RefundService = RefundService;
            this.TransferService = TransferService;
        }

        public Task<SettlementRollbackStatus> Create(SettlementRollbackCreateArgument Argument)
        {
            Argument.ClientInfo = Argument.ClientInfo ?? ServiceContext.ClientService.GetClientInfo();
            Ensure.Assert(Argument.Amount > 0, "退回金额必须大于0");

            return DataScope.Use("创建结算回退记录", async ctx =>
            {
                var settlementRecord = await ctx.Queryable<DataModels.DataSettlementRecord>(false)
                  .Where(r => r.Id == Argument.SettlementId && r.LogicState == EntityLogicState.Enabled)
                  .Include(r => r.Items)
                  .SingleOrDefaultAsync();
                if (settlementRecord == null)
                    throw new PublicArgumentException("找不到结算记录:" + Argument.SettlementId);

                if (settlementRecord.LeftAmount < Argument.Amount)
                    throw new PublicArgumentException($"剩余金额{settlementRecord.LeftAmount}少于退回金额{Argument.Amount},结算记录:{settlementRecord.Id}");

                var rollbackId = await IdentGenerator.GenerateAsync<DataSettlementRollbackRecord>();

                var rollbackItems = new List<DataSettlementRollbackItemRecord>();
                var leftAmount = Argument.Amount;
                
                foreach(var item in settlementRecord.Items)
                {
                    if (item.LeftAmount == 0)
                        continue;

                    var amountUsed = Math.Min(leftAmount, item.LeftAmount);
                    rollbackItems.Add(new DataSettlementRollbackItemRecord
                    {
                        Id = await IdentGenerator.GenerateAsync<DataSettlementRollbackItemRecord>(),
                        Amount = amountUsed,
                        SettlementItemRecordId = item.Id,
                        SettlementRollbackRecordId = rollbackId,
                        TitleId=item.TitleId
                    });
                    item.LeftAmount -= amountUsed;
                    item.RollbackAmount += amountUsed;
                    ctx.Update(item);

                    settlementRecord.LeftAmount -= amountUsed;
                    settlementRecord.RollbackAmount += amountUsed;

                    leftAmount -= amountUsed;
                    if (leftAmount == 0)
                        break;
                }
                ctx.Update(settlementRecord);

                if (leftAmount > 0)
                    throw new InvalidOperationException($"剩余结算金额不足回退的金额，结算记录:{settlementRecord.Id},退回金额:{Argument.Amount}");

                var record = new DataSettlementRollbackRecord
                {
                    Amount = Argument.Amount,
                    BizParent = Argument.BizParent,
                    BizRoot = Argument.BizRoot,
                    BuyerId = settlementRecord.BuyerId,
                    CreatedTime = Now,
                    Desc = Argument.Desc,
                    Id = rollbackId,
                    Name = Argument.Name,
                    OpAddress = Argument.ClientInfo.ClientAddress,
                    OpDevice = Argument.ClientInfo.DeviceType,
                    OwnerId = Argument.ClientInfo.OperatorId,
                    SellerId = settlementRecord.SellerId,
                    SettlementRecordId = settlementRecord.Id,
                    UpdatedTime = Now,
                    UpdatorId = Argument.ClientInfo.OperatorId,
                    Items = rollbackItems
                };
                var ids=await TransferService.Value.Transfer(new TransferArgument
                {
                    ClientInfo = Argument.ClientInfo,
                    Items = rollbackItems.Select((i,idx) => new TransferArgumentItem
                    {
                         Amount=i.Amount,
                         BizParent=new TrackIdent("退款","SettlementRollbackItemRecord",i.Id),
                         BizRecordIndex=idx,
                         BizRoot=Argument.BizRoot,
                         Description=Argument.Desc,
                         DstId=settlementRecord.BuyerId,
                         DstTitleId=i.TitleId,
                         SrcId=settlementRecord.SellerId,
                         SrcTitleId=settlementRecord.PrepayTitleId
                    }).ToArray()
                });
                foreach (var pair in rollbackItems.Zip(ids, (i, id) => (i, id)))
                    pair.i.TransferRecordId = pair.id;

                ctx.Add(record);


                DateTime? Expires = null;
                var refundRollbackItem = rollbackItems.SingleOrDefault(i => i.TitleId == settlementRecord.CollectTitleId);
                if (refundRollbackItem!=null)
                {
                    //如果支付平台需要退款
                    record.RefundPlatformPlatformId = settlementRecord.CollectPaymentPlatformId;
                    record.RefundTitleId = refundRollbackItem.TitleId;
                    record.RefundAmount = refundRollbackItem.Amount;

                    var re=await RefundService.Value.Create(new RefundRequest
                    {
                        Amount = refundRollbackItem.Amount,
                        BizParent = new TrackIdent("退款", "SettlementRollbackRecord", record.Id),
                        BizRoot = Argument.BizRoot,
                        ClientInfo = Argument.ClientInfo,
                        CollectIdent = settlementRecord.CollectRecordId.Value,
                        Title = Argument.Name,
                        Desc = Argument.Desc,
                        PaymentPlatformId = settlementRecord.CollectPaymentPlatformId.Value
                    });
                    record.RefundRecordId = re.Id;
                    await UpdateRecordState(ctx, record, re, Argument.ClientInfo);
                    Expires = re.Expires;
                }
                else
                {
                    record.State = SettlementRollbackState.Success;
                }

                await ctx.SaveChangesAsync();

                return new SettlementRollbackStatus
                {
                    Id=record.Id,
                    State=record.State,
                    Message=record.Error,
                    Expires=Expires
                };
            });
        }
        async Task UpdateRecordState(IDataContext ctx,DataSettlementRollbackRecord record,RefundRefreshResult re,ClientInfo clientInfo)
        {
            if (re.State != RefundState.Failed && re.State != RefundState.Success)
            {
                record.State = SettlementRollbackState.Processing;
                return;
            }

            if (re.State == RefundState.Failed)
            {
                record.State = SettlementRollbackState.Failed;
                record.Error = re.Error;
                record.UpdatedTime = Now;
                record.UpdatorId = clientInfo.OperatorId.Value;
            }

            record.State = SettlementRollbackState.Success;
            record.UpdatedTime = Now;
            record.UpdatorId = clientInfo.OperatorId.Value;
            var updater = new AccountUpdater(ctx, IdentGenerator);
            await updater.LoadAccounts((record.RefundTitleId.Value, record.BuyerId));
            await updater.Update(record.RefundTitleId.Value, record.BuyerId, 0, record.RefundAmount.Value, Now);
        }
        async Task UpdateRefundStatus(IDataContext ctx,DataSettlementRollbackRecord record,ClientInfo clientInfo)
        {
            var re = await RefundService.Value.RefreshRefundRecord(record.RefundRecordId.Value);
            await UpdateRecordState(ctx, record, re, clientInfo);

        }

        public Task<SettlementRollbackStatus> UpdateAndQueryStatus(long Id,ClientInfo ClientInfo)
        {
            ClientInfo = ClientInfo ?? ServiceContext.ClientService.GetClientInfo();

            return DataScope.Use("更新结算退款记录", async ctx =>
            {
                var record = await ctx.Queryable<DataModels.DataSettlementRollbackRecord>(false)
                    .Where(r=>r.Id==Id)
                    .SingleOrDefaultAsync();
                if (record == null)
                    throw new PublicArgumentException("找不到结算记录:" + Id);


                if (record.State != SettlementRollbackState.Processing)
                    return new SettlementRollbackStatus
                    {
                        Id = Id,
                        State = record.State,
                        Message=record.Error
                    };

                if (record.RefundRecordId.HasValue)
                    await UpdateRefundStatus(ctx, record, ClientInfo);
                else
                    record.State = SettlementRollbackState.Success;

                return new SettlementRollbackStatus
                {
                    Id = Id,
                    State = record.State,
                    Message=record.Error
                };
            });
        }
    }

}
