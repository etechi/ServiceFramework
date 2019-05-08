using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SF.Biz.Payments
{


    /// <summary>
    /// 收款结果
    /// </summary>
    public class CollectResult
    {
        public HttpResponseMessage HttpResponse { get; set; }
        public CollectSession Session { get; set; }
    }
    /// <summary>
    /// 收款启动结果
    /// </summary>
    public class CollectStartStatus
    {
        public IDictionary<string, string> Result { get; set; }
        public string ExtraData { get; set; }
        public string ExtIdent { get; set; }
    }
    public interface ICollectStartArgumentLoader
    {
        Task<CollectStartArgument> GetRequest(long ident);
    }


    public interface ICollectProvider
	{
		string Name { get; }
        TimeSpan? CollectRequestTimeout { get; }
		Task<CollectStartStatus> Start(
            long ident,
            CollectStartArgument StartArgument, 
            StartRequestInfo Request,
            string CallbackUrl,
            string NotifyUrl
            );
		Task<CollectResult> GetResultByNotify(ICollectStartArgumentLoader StartArgumentLoader);
		Task<CollectResult> GetResultByCallback(ICollectStartArgumentLoader StartArgumentLoader);
		Task<CollectResponse> GetResultByQuery(CollectStartArgument StartArgument);

	}

}
