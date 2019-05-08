using SF.Biz.Payments;
using SF.Sys;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Events;
using SF.Sys.Logging;
using SF.Sys.NetworkService;
using SF.Sys.Reminders;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{

    public abstract class Argument
    {
        public abstract IAccountService AccountService { get; }
        Lazy<IAccountManager> AccountManager { get; }

        Lazy<IDepositService> DepositService { get; }
        Lazy<IDepositRecordManager> DepositRecordManager { get; }
        Lazy<IAccountTitleManager> TitleService { get; }
        IAccessToken AccessToken { get; }
        Lazy<IInvokeContext> InvokeContext { get; }
        Lazy<IClientService> ClientService { get; }
        Lazy<ITimeService> TimeService { get; }
        Lazy<IRemindService> RemindService { get; }
        Lazy<IPaymentPlatformService> PaymentPlatformService { get; }
    }
    /// <summary>
    /// 账户充值记录
    /// </summary>
    public class AccountingService:
		IAccountingService
	{
        Lazy<IAccountService> AccountService { get; }
        Lazy<IAccountManager> AccountManager { get; }

        Lazy<IDepositService> DepositService { get; }
        Lazy<IDepositRecordManager> DepositRecordManager { get; }
        Lazy<IAccountTitleManager> TitleService { get; }
        IAccessToken AccessToken { get; }
        Lazy<IInvokeContext> InvokeContext { get; }
        Lazy<IClientService> ClientService { get; }
        Lazy<ITimeService> TimeService { get;  }
        Lazy<IRemindService> RemindService { get; }
        Lazy<IPaymentPlatformService> PaymentPlatformService { get; }


        long EnsureUserIdent()
            => AccessToken.User.EnsureUserIdent();

        
        public AccountingService(
            Lazy<IAccountService> AccountService,
            Lazy<IAccountManager> AccountManager,
            Lazy<IDepositService> DepositService,
            Lazy<IAccountTitleManager> TitleService,
            Lazy<IDepositRecordManager> DepositRecordManager,
            Lazy<IInvokeContext> InvokeContext,
            Lazy<ITimeService> TimeService,
            Lazy<IClientService> ClientService,
            IAccessToken AccessToken,
            Lazy<IRemindService> RemindService,
            Lazy<IPaymentPlatformService> PaymentPlatformService
            )
        {
            this.InvokeContext = InvokeContext;
            this.AccountService = AccountService;
            this.AccountManager = AccountManager;
            this.TitleService = TitleService;
            this.AccessToken = AccessToken;
            this.DepositService = DepositService;
            this.DepositRecordManager = DepositRecordManager;
            this.TimeService = TimeService;
            this.ClientService = ClientService;
            this.RemindService = RemindService;
            this.PaymentPlatformService = PaymentPlatformService;
        }

        public async Task<DepositRecord> GetDepositRecord(long id)
        {
            var uid = EnsureUserIdent();
            var re = await this.DepositRecordManager.Value.GetAsync(ObjectKey.From(id));
            if (re.DstId != uid)
                return null;
            return re;
        }
        
        public async Task<QueryResult<DepositRecord>> QueryDepositRecords(ClientDepositRecordQueryArguments Args)
        {
            var uid = EnsureUserIdent();
            var iargs = new DepositRecordQueryArguments();
            iargs.Paging = Args.Paging;
            iargs.OwnerId = uid;
            iargs.Id = Args.Id;
            iargs.State = Args.State;
            iargs.PaymentPlatformId = Args.PaymentPlatformId;
            iargs.Time = Args.BeginTime.HasValue || Args.EndTime.HasValue ? new DateQueryRange { Begin = Args.BeginTime, End = Args.EndTime } : null;

            return await this.DepositRecordManager.Value.QueryAsync(iargs);
        }

        public async Task<decimal> Balance()
        {
            var uid = EnsureUserIdent();
            return await this.AccountService.Value.GetSettlementBalance(uid);

        }
        
        public async Task<DepositRecord> RefreshDepositRecord(long Id)
        {
            var uid = EnsureUserIdent();
            var re = await DepositService.Value.GetDepositResult(Id,true,true);
            if (re.DstId != uid)
                return null;
            return re;
        }

        public async Task<DepositStartResult> Deposit(ClientDepositArguments Args)
        {
            var uid = EnsureUserIdent();

            var did = await DepositService.Value.CreateDeposit(
                new DepositArgument
                {
                    Amount = Args.Amount,
                    AccountTitle = "balance",
                    Description = "充值",
                    DstId = uid,
                    OperatorId = uid,
                    PaymentPlatformId = Args.PaymentPlatformId,
                    ClientType = Args.ClientType,
                    OpAddress = ClientService.Value.UserAgent.Address,
                    OpDevice = ClientService.Value.UserAgent.DeviceType,
                    HttpRedirest = Args.Redirect,
                    TrackEntityIdent = "充值操作-" + uid + "-" + TimeService.Value.Now.ToString("yyyyMMddHHmmss")
                });

            await RemindService.Value.Setup(new RemindSetupArgument
            {
                BizIdent = did,
                BizIdentType = "DepositRecord",
                BizType = "充值",
                Name = "充值",
                RemindableName = typeof(DepositRemindable).FullName,
                RemindTime = DepositRemindable.GetEndTime(PaymentPlatformService.Value,Args.PaymentPlatformId,TimeService.Value.Now)
            });

            var re = await DepositService.Value.StartDeposit(
                did,
                new Payments.StartRequestInfo
                {
                   ClientAddress = ClientService.Value.UserAgent.Address,
                    DeviceType = ClientService.Value.UserAgent.DeviceType,
                    Uri = InvokeContext.Value.Request.GetUri()
                }
            );
            return re;
        }
    }
}
