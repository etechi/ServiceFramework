using SF.Sys;
using SF.Sys.Collections.Generic;
using SF.Sys.NetworkService;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SF.Biz.Payments.Platforms.Weichat
{
    public interface IUserOpenIdProvider
	{
		Task<string> GetOpenIdByUserId(string UserId);
    }
    /// <summary>
    /// 微信收款提供者
    /// </summary>
    public class WeichatCollectProvider : ICollectProvider,ICollectQrCodeResolver
	{
		public ITimeService TimeService { get; }
		public Lazy<IUserOpenIdProvider> UserOpenIdProvider { get; }
		public WxPayAPI.WxPayConfig Config { get; set; }
        public IInvokeContext InvokeContext { get; set; }
        public TimeSpan? CollectRequestTimeout => TimeSpan.FromHours(2);
        WxPayAPI.WxPayApi WxPayApi { get; }
		public WeichatCollectProvider(
			 WeiChatPaySetting Setting,
			ITimeService TimeService, 
			Lazy<IUserOpenIdProvider> UserOpenIdProvider,
            IInvokeContext InvokeContext
            )
		{
			Config = new WxPayAPI.WxPayConfig
			{
				APPID = Setting.AppId,
				KEY = Setting.KEY,
				MCHID = Setting.MCHID,
				SSLCERT_PASSWORD = Setting.SSLCERT_PASSWORD,
				SSLCERT_PATH = Setting.SSLCERT_PATH
			};
			WxPayApi = new WxPayAPI.WxPayApi { WxPayConfig = Config };
			this.TimeService = TimeService;
			this.UserOpenIdProvider = UserOpenIdProvider;
            this.InvokeContext = InvokeContext;

        }
		public string Title
		{
			get { return "微信支付"; }
		}

		public Task<CollectResponse> GetResultByQuery(CollectStartArgument StartArgument)
		{
			var r = new CollectResponse();
			return Task.FromResult(r);
		}

		public class ExtraData
		{
			public string qr { get; set; }
		}

		public async Task<CollectStartStatus> Start(long Ident,CollectStartArgument StartArgument, StartRequestInfo Request, string callback_url, string notify_url)
		{
			var data = new WxPayAPI.WxPayData();

			data.SetValue("body", StartArgument.Title);
			data.SetValue("detail", StartArgument.Title);
			data.SetValue("out_trade_no", Ident);
			data.SetValue("total_fee", ((int)(StartArgument.Amount * 100)).ToString());
			data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
			data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));

			data.SetValue("spbill_create_ip",Request.ClientAddress);
			data.SetValue("notify_url", notify_url);
			//data.SetValue("goods_tag", "test");
			data.SetValue("trade_type", StartArgument.ClientType=="weichat"?"JSAPI": StartArgument.ClientType == "app"?"APP":"NATIVE");
			//data.SetValue("notify_url", StartArgument.NotifyUrl);
			//微信支付必须提供openid
			if (StartArgument.ClientType=="weichat")
				data.SetValue("openid",await UserOpenIdProvider.Value.GetOpenIdByUserId(StartArgument.CurUserId.ToString()));

			//data.SetValue("openid", openid);
			//扫码支付必须提供产品ID
			if (StartArgument.ClientType == "desktop")
				data.SetValue("product_id", StartArgument.TrackEntityIdent);

			var re = WxPayApi.UnifiedOrder(data);
			if (re.GetValue("return_code") as string != "SUCCESS")
				throw new Exception(re.GetValue("return_msg") as string ?? "未知错误");


			if (re.GetValue("result_code") as string != "SUCCESS")
			{
				//if (re.GetValue("err_code") as string == "OUT_TRADE_NO_USED")
				//{
				//	var queryOrderInput = new WxPayAPI.WxPayData();
				//	queryOrderInput.SetValue("out_trade_no", sequence);
				//	var query_result = WxPayAPI.WxPayApi.OrderQuery(queryOrderInput);
				//	if(	query_result.GetValue("result_code") as string =="SUCCESS" && 
				//		query_result.GetValue("return_code") as string == "SUCCESS" && 
				//		query_result.GetValue("trade_state") as string == "NOTPAY")
				//	{

				//	}
				//}
				throw new Exception((re.GetValue("err_code") as string ?? "-1") + (re.GetValue("err_code_des") as string ?? "未知错误"));
			}

			//公众账号ID appid String(32) 是 wx8888888888888888 微信分配的公众账号ID
			//商户号 partnerid String(32) 是 1900000109 微信支付分配的商户号
			//预支付交易会话ID prepayid String(32) 是 WX1217752501201407033233368018 微信返回的支付交易会话ID
			//扩展字段 package String(128) 是 Sign = WXPay 暂填写固定值Sign = WXPay
			//随机字符串 noncestr String(32) 是 5K8264ILTKCH16CQ2502SI8ZNMTM67VS 随机字符串，不长于32位。推荐随机数生成算法
			//时间戳 timestamp String(10) 是 1412000000 时间戳，请见接口规则 - 参数规定
			//签名 sign String(32) 是 C380BEC2BFD727A4B6845133519F3AD6 签名，详见签名生成算法

			var res = new WxPayAPI.WxPayData();
			res.SetValue("appid", Config.APPID);
			res.SetValue("partnerid", Config.MCHID);
			res.SetValue("prepayid", re.GetValue("prepay_id"));
			res.SetValue("package", "Sign=WXPay");
			res.SetValue("noncestr", this.WxPayApi.GenerateNonceStr());
			res.SetValue("timestamp", this.WxPayApi.GenerateTimeStamp());
			res.SetValue("sign", res.MakeSign(Config));

			var result = new Dictionary<string, string>();
			if (StartArgument.ClientType == "weichat")
				result["data"] = Json.Stringify(res.GetValues());
			else
				result["redirect"] = "/payment/qrcode/" + Ident;

			var code_url = Convert.ToString(re.GetValue("code_url"));
			return new CollectStartStatus
			{
				Result = result,
				ExtraData = string.IsNullOrEmpty(code_url) ? null :  Json.Stringify(new ExtraData { qr= code_url })
			};
		}


		public string GetQrCode(string ExtraData)
		{
			if (string.IsNullOrWhiteSpace(ExtraData)) return null;
			var ed = Json.Parse<ExtraData>(ExtraData);
			return ed.qr;
		}
		public async Task<CollectResult> GetResultByNotify(ICollectStartArgumentLoader StartArgumentLoader)
		{
			var r = new CollectResponse();
			var args = InvokeContext.Request.GetQueryAsDictionary();
            r.CompletedTime = TimeService.Now;
			r.Ident = args.Get("id").ToInt64();
			r.AmountCollected = decimal.Parse(args.Get("amount"));
			r.ExtIdent = "EXT-" + r.Ident;
			r.ExtUserId = args.Get("user-id");
			r.ExtUserName = args.Get("user-name");
			r.ExtUserAccount = args.Get("user-account");

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
			r.ExtUserId = args.Get("user-id");
			r.ExtUserName = args.Get("user-name");
			r.ExtUserAccount = args.Get("user-account");

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
