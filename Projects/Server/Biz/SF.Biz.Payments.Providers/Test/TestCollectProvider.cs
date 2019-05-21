using SF.Sys;
using SF.Sys.Clients;
using SF.Sys.Collections.Generic;
using SF.Sys.NetworkService;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Biz.Payments.Platforms.Tests
{
    /// <summary>
    /// 测试收款提供者
    /// </summary>
    [SupportedDeviceTypes(new[] { ClientDeviceType.WAP, ClientDeviceType.PCBrowser, ClientDeviceType.WinXin})]
    public class TestCollectProvider : ICollectProvider,ICollectQrCodeResolver,IRefundProvider
	{
		public ITimeService TimeService { get; }
        public IInvokeContext InvokeContext { get; }
        public TimeSpan? CollectRequestTimeout => null;// TimeSpan.FromMinutes(1);
        public IServiceInstanceDescriptor ServiceInstanceDescriptor { get; }
        public TestCollectProvider(ITimeService TimeService, IInvokeContext InvokeContext, IServiceInstanceDescriptor ServiceInstanceDescriptor)
        {
			this.TimeService = TimeService;
            this.InvokeContext = InvokeContext;
            this.ServiceInstanceDescriptor = ServiceInstanceDescriptor;

        }
		public string Title
		{
			get { return ServiceInstanceDescriptor.Meta.Title; }
		}

		public Task<CollectResponse> GetResultByQuery(CollectStartArgument StartArgument)
		{
			var r = new CollectResponse();
			return Task.FromResult(r);
		}

		public Task<CollectStartStatus> Start(long Ident, CollectStartArgument StartArgument,ClientInfo request, string callbackUrl, string notifyUrl)
		{
			var re = new Dictionary<string, string>();
			re["redirect"] = $"/Test/TestCollectPayment/{Ident}?amount={StartArgument.Amount}&callback={Uri.EscapeDataString(callbackUrl)}";
			re["id"] = Ident.ToString();
            re["unittest-notify"] = $"http://localhost/api/collectcallback/{StartArgument.PaymentPlatformId}?id={Ident}&amount={StartArgument.Amount}&user-id={StartArgument.ClientInfo.OperatorId}&user-name=name-{StartArgument.ClientInfo.OperatorId}&user-account=account-{StartArgument.ClientInfo.OperatorId}";
            return Task.FromResult(
				new CollectStartStatus {
                    Expires=TimeService.Now.AddDays(1),
					Result = re,
					ExtraData = Json.Stringify(
						new ExtraData {
							qr = "http://"+Ident
						}
						)
					}
				);
		}

		public async Task<CollectResult> GetResultByNotify(ICollectStartArgumentLoader StartArgumentLoader)
		{
			var r = new CollectResponse();
            var args = InvokeContext.Request.GetQueryAsDictionary();
			r.CompletedTime = TimeService.Now;
			r.Ident = args.Get("id").ToInt64();
            r.AmountCollected = args.Get("amount").ToDecimal();
			r.ExtIdent = "EXT-" + r.Ident;
			r.ExtUserId = args.Get("user-id");
			r.ExtUserName = args.Get("user-name");
			r.ExtUserAccount = args.Get("user-account");

			var req = await StartArgumentLoader.GetRequest(r.Ident);

            var httpresponse = HttpResponse.Text("OK");
			return new CollectResult
			{
				Session = new CollectSession
				{
					Request = req,
					Response = r
				},
				HttpResponse = httpresponse
			};
		}

		public async Task<CollectResult> GetResultByCallback( ICollectStartArgumentLoader StartArgumentLoader)
		{
			var r = new CollectResponse();
            var args = InvokeContext.Request.GetQueryAsDictionary();
			r.CompletedTime = TimeService.Now;
			r.Ident = args.Get("pid").ToInt64(); 
			r.AmountCollected = args.Get("amount").ToDecimal();
			r.ExtIdent = "EXT-" + r.Ident;
			r.ExtUserId = args.Get("user-id");
			r.ExtUserName = args.Get("user-name");
			r.ExtUserAccount = args.Get("user-account");

			var req = await StartArgumentLoader.GetRequest(r.Ident);

			//var httpresponse = Request.CreateResponse(System.Net.HttpStatusCode.Redirect);

			//httpresponse.Headers.Location = new Uri(Request.RequestUri, req.HttpRedirect);
			return new CollectResult
			{
				Session = new CollectSession
				{
					Request = req,
					Response = r
				},
				HttpResponse = HttpResponse.JSRedirectWithoutHistory(
                    new Uri(InvokeContext.Request.GetUri(), req.HttpRedirect)
                    )
			};
		}
		class ExtraData
		{
			public string qr { get; set; }
		}
		public string GetQrCode(string ExtraData)
		{
			if (string.IsNullOrWhiteSpace(ExtraData)) return null;
			var ed = Json.Parse<ExtraData>(ExtraData);
			return ed.qr;

		}

        Task<RefundResponse> IRefundProvider.TryRefund(long Ident,RefundStartArgument StartArgument)
        {
            var time = TimeService.Now;
            var past = time.Subtract(StartArgument.SubmitTime).TotalSeconds / 60;
            var state = past > 3 ? RefundState.Success :
                    RefundState.Processing;

            return Task.FromResult(new RefundResponse
            {
                Ident=Ident,
                RefundAmount=StartArgument.Amount,
                State=state,
                UpdatedTime=time,
            });

        }
    }


}
