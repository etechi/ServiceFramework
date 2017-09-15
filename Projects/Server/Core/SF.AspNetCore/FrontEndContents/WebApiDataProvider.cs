using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Reflection;
using System.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Core;

namespace SF.Management.FrontEndContents.Mvc
{
 //   [Comment(Name = "界面管理器WebApi数据提供者")]
 //   public class WebApiDataProvider : IDataProvider
	//{
	//	public IDIScope DIScope { get; }
	//	public ILogger Logger { get; }
	//	public WebApiDataProvider(IDIScope DIScope,ILogService LogService)
	//	{
	//		this.DIScope = DIScope;
	//		this.Logger = LogService.GetLogger(GetType());
	//	}
	//	static HttpRoute EmptyRoute { get; } = new HttpRoute();

	//	public Task<object> Load(IContent content, string contentConfig)
	//	{
	//		return Load(content.ProviderConfig, contentConfig);
	//	}
	//	public async Task<object> Load(string contentConfig,string blockConfig)
	//	{
	//		if (string.IsNullOrWhiteSpace(contentConfig))
	//			return Task.FromResult<object>(null);

	//		var cfgs = Json.Parse<Dictionary<string, string>>(contentConfig);
	//		if (!string.IsNullOrWhiteSpace(blockConfig))
	//		{
	//			var block_cfgs = Json.Parse<Dictionary<string, string>>(blockConfig);
	//			foreach (var p in block_cfgs)
	//				cfgs[p.Key] = p.Value;
	//		}

	//		string controller;
	//		string action;
	//		if (!cfgs.TryGetValue("controller", out controller))
	//			throw new ArgumentException("参数缺少控制器");
	//		if (!cfgs.TryGetValue("action", out action))
	//			throw new ArgumentException("参数缺少动作");
	//		var query = cfgs
	//			.Where(p => p.Key != "controller" && p.Key != "action")
	//			.Select(p => p.Key + "=" + Uri.EscapeDataString(p.Value))
	//			.Join("&");
	//		var uri = $"http://localhost/{controller}/{action}?{query}";

	//		var rd = new HttpRouteData(EmptyRoute);
	//		rd.Values["controller"] = controller;
	//		rd.Values["action"] = action;

	//		var ctx = new HttpRequestContext
	//		{
	//			RouteData = rd
	//		}; ControllerActionExecutor
	//		using (var dispatcher = new ControllerDispatcher(System.Web.Http.GlobalConfiguration.Configuration))
	//		{
	//			var req = new HttpRequestMessage();
	//			req.Method = HttpMethod.Get;
	//			req.RequestUri = new Uri(uri);

	//			return await dispatcher.GetActionResultAsync(req, ctx);
	//		}
	//	}
	//}

}
