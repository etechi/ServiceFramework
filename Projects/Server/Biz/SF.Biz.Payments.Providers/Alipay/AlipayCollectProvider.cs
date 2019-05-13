using SF.Sys;
using SF.Sys.Collections.Generic;
using SF.Sys.NetworkService;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Biz.Payments.Platforms.Alipay
{
    /// <summary>
    /// 支付宝收款提供者
    /// </summary>

    public class AlipayCollectProvider : ICollectProvider
	{
		public ITimeService TimeService { get; }
        public IInvokeContext InvokeContext { get; }
        public AlipayCollectProvider(ITimeService TimeService, IInvokeContext InvokeContext)
		{
			this.TimeService = TimeService;
            this.InvokeContext = InvokeContext;
        }
        public string Title
		{
			get { return "支付宝"; }
		}
        public TimeSpan? CollectRequestTimeout => null;
        public Task<CollectResponse> GetResultByQuery(CollectStartArgument StartArgument)
		{
			var r = new CollectResponse();
			return Task.FromResult(r);
		}

		public Task<CollectStartStatus> Start(long Ident,CollectStartArgument StartArgument, StartRequestInfo requestInfo, string callbackUrl,string notifyUrl)
		{
			var re = new Dictionary<string, string>();
			re["redirect"] = $"/Test/TestCollectPayment/{Ident}?amount={StartArgument.Amount}&callback={Uri.EscapeDataString(callbackUrl)}";
			return Task.FromResult(new CollectStartStatus { Result = re });
		}

		public async Task<CollectResult> GetResultByNotify(ICollectStartArgumentLoader StartArgumentLoader)
		{
			var r = new CollectResponse();
			var args = InvokeContext.Request.GetQueryAsDictionary();
            r.CompletedTime = TimeService.Now;
			r.Ident = args.Get("id").ToInt64(); 
			r.AmountCollected = decimal.Parse(args.Get("amount"));
			r.ExtIdent = "EXT-" + r.Ident;
			r.ExtUserId = args.Get("user-id") ?? "user-id";
			r.ExtUserName = args.Get("user-name") ?? "user-name";
			r.ExtUserAccount = args.Get("user-account") ?? "user-account";

			var req = await StartArgumentLoader.GetRequest(r.Ident);

			var httpresponse = HttpResponse.Redirect(new Uri(req.HttpRedirect));
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

		public async Task<CollectResult> GetResultByCallback(ICollectStartArgumentLoader StartArgumentLoader)
		{
			var r = new CollectResponse();
			var args = InvokeContext.Request.GetQueryAsDictionary();
            r.CompletedTime = TimeService.Now;
			r.Ident = args.Get("id").ToInt64();
			r.AmountCollected = decimal.Parse(args.Get("amount"));
			r.ExtIdent = "EXT-" + r.Ident;
			r.ExtUserId = args.Get("user-id") ?? "user-id";
			r.ExtUserName = args.Get("user-name") ?? "user-name";
			r.ExtUserAccount = args.Get("user-account") ?? "user-account";

			var req = await StartArgumentLoader.GetRequest(r.Ident);

			var httpresponse = HttpResponse.Redirect(new Uri(req.HttpRedirect));
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

	}


}
