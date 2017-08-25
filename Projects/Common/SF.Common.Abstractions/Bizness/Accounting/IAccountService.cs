using SF.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Bizness.Accounting
{
	public class TransferArgumentItem
	{
		public string SrcTitle { get; set; }
		public string DstTitle { get; set; }
		public int SrcId { get; set; }
		public int DstId { get; set; }
		public decimal Amount { get; set; }
		public string Description { get; set; }
        public string TrackEntityIdent { get; set; }
        public int BizRecordIndex { get; set; }
        //public TransferArgumentItem<TUserKey>[] Children { get; set; }
    }
	public class TransferArgument
	{
		public TransferArgumentItem[] Items { get; set; }
		public int OperatorId { get; set; }
        public string OpAddress { get; set; }
        public DeviceType OpDevice { get; set; }
    }
    public class DepositArgument
	{
		public string AccountTitle { get; set; }
		public int DstId { get; set; }
		public int PaymentPlatformId { get; set; }
		public int OperatorId { get; set; }
		public decimal Amount { get; set; }
		public string Description { get; set; }
		public string TrackEntityIdent { get; set; }
		public string CallbackName { get; set; }
		public string CallbackContext { get; set; }
		public string ClientType { get; set; }
		public string HttpRedirest { get; set; }
        public string OpAddress { get; set; }
        public DeviceType OpDevice { get; set; }
	}
    public class RefundArgument
    {
        public int DepositRecordId { get; set; }
        public int OperatorId { get; set; }
        public decimal Amount { get; set; }
        public string AccountTitle { get; set; }
        public string Description { get; set; }
        public string TrackEntityIdent { get; set; }
        public string CallbackName { get; set; }
        public string CallbackContext { get; set; }
        public string OpAddress { get; set; }
        public DeviceType OpDevice { get; set; }
        public string Reason { get; set; }
    }
    public class SettlementArgument
    {
        public string FirstTitle { get; set; }
        public int SrcId { get; set; }
        public string DstTitle { get; set; }
        public int DstId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string TraceEntityIdent { get; set; }
        public int OperatorId { get; set; }
        public string OpAddress { get; set; }
        public DeviceType OpDevice { get; set; }
    }

    public class DepositDone: 
		ServiceProtocol.CallGuarantors.ICallable
	{
		IAccountService AccountService { get; }
		public DepositDone(IAccountService AccountService)
		{
			this.AccountService = AccountService;
		}
		public async Task Execute(string Argument, string Context, Exception Exception)
		{
			int rid = int.Parse(Context);
			await AccountService.CompleteDeposit(rid, Argument, Exception);
		}
	}
   
    public class DepositStartResult
	{
        public int Id { get; set; }
		public string RecordId { get; set; }
		public IDictionary<string,string> PaymentStartResult { get; set; }
	}

    public class DepositRecordQueryArguments
    {
        [Display(Name="充值编号")]
        public string Ident { get; set; }

        [Display(Name = "用户")]
        [EntityIdent("用户")]
        public int? OwnerId { get; set; }

        [Display(Name ="充值时间")]
        public DateQueryRange Time { get; set; }

        [Display(Name = "充值平台")]
        [EntityIdent("系统服务", ScopeValue ="常规充值")]
        public int? PaymentPlatformId { get; set; }

        [Display(Name="科目")]
        [EntityIdent("账户科目")]
        public int? TitleId { get; set; }

        [Display(Name = "充值状态")]
        public DepositState? State { get; set; }

        [Display(Name = "金额")]
        public QueryRange<decimal> Amount { get; set; }
    }

    public class RefundStartResult
    {
        public string RecordId { get; set; }
        public IDictionary<string, string> PaymentStartResult { get; set; }
    }

    public class RefundRecordQueryArguments
    {
        [Display(Name = "充值编号")]
        public string DepositIdent { get; set; }

        [Display(Name = "退款编号")]
        public string RefundIdent { get; set; }

        [Display(Name = "用户")]
        [EntityIdent("用户")]
        public int? OwnerId { get; set; }

        [Display(Name = "退款时间")]
        public DateQueryRange Time { get; set; }

        [Display(Name = "充值平台")]
        [EntityIdent("系统服务", ScopeValue = "常规充值")]
        public int? PaymentPlatformId { get; set; }

        [Display(Name = "科目")]
        [EntityIdent("账户科目")]
        public int? TitleId { get; set; }

        [Display(Name = "退款状态")]
        public RefundState? State { get; set; }
        [Display(Name = "金额")]
        public QueryRange<decimal> Amount { get; set; }
    }

    public class TransferRecordQueryArguments
    {
        [Display(Name = "转账ID")]
        public int? Id { get; set; }

        [Display(Name = "源用户")]
        [EntityIdent("用户")]
        public int? SrcId { get; set; }

        [Display(Name = "源科目")]
        [EntityIdent("账户科目")]
        public int? SrcTitleId { get; set; }

        [Display(Name = "目标用户")]
        [EntityIdent("用户")]
        public int? DstId { get; set; }

        [Display(Name = "目标科目")]
        [EntityIdent("账户科目")]
        public int? DstTitleId { get; set; }

        [Display(Name = "时间")]
        public DateQueryRange Time { get; set; }
        [Display(Name = "金额")]
        public QueryRange<decimal> Amount { get; set; }
    }
    public class AccountQueryArguments
    {
        [Display(Name = "用户")]
        [EntityIdent("用户")]
        public int? OwnerId { get; set; }

        [Display(Name = "科目")]
        [EntityIdent("账户科目")]
        public int? TitleId { get; set; }

        [Display(Name = "更新时间")]
        public DateQueryRange UpdatedTime { get; set; }

        [Display(Name = "余额")]
        public QueryRange<decimal> Amount { get; set; }

        [Display(Name = "转入数额")]
        public QueryRange<decimal> Inbound { get; set; }

        [Display(Name = "转出数额")]
        public QueryRange<decimal> Outbound { get; set; }

    }
    public interface IAccountService
	{
     

        #region 转账
        Task<int[]> Settlement(SettlementArgument Arg);
        Task<int[]> Transfer(TransferArgument Arg);
        Task<QueryResult<TransferRecord>> ListTransferRecord(
            TransferRecordQueryArguments args,
            Paging Paging
            );
        Task<TransferRecord> GetTransferRecord(int Id);
        #endregion

        #region 账户
        Task<long> GetTypeId(string Title);
        Task<decimal> GetBalance(int OwnerId, string Type);
        Task<Account> GetAccount(int TitleId,int OwnerId);
        Task<decimal> GetSettlementBalance(int OwnerId);
        Task<Dictionary<int, decimal>> GetSettlementBalances(int[] OwnerIds);

        Task<AccountInternal> GetAccountInternal(int TitleId, int OwnerId);
        Task<QueryResult<AccountInternal>> ListAccountInternals(
            AccountQueryArguments args,
            Paging Paging
            );
        #endregion
    }
}
