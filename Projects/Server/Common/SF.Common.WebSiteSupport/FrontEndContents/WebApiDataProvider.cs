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
