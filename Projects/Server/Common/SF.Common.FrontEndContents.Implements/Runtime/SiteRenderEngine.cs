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
using System.Threading.Tasks;
using System.Collections.Concurrent;
using SF.Sys.Logging;
using SF.Sys.Events;
using SF.Sys.Entities;
using SF.Sys.Services;
using SF.Sys.NetworkService;

namespace SF.Common.FrontEndContents.Runtime
{
	public class SiteLoadContext
	{
		public List<string> Messages { get; } = new List<string>();
	}
	public class SiteRenderEngine :
		SiteRenderEngine<Content, SF.Common.FrontEndContents.Site, SiteTemplate>
	{
		public SiteRenderEngine(
			ILogService LogService, 
			IEventSubscriber<EntityChanged<Content>> OnContentModified, 
			IEventSubscriber<EntityChanged<FrontEndContents.Site>> OnSiteTemplateModified, 
			IEventSubscriber<EntityChanged<SiteTemplate>> OnSiteModified
			) : base(LogService, OnContentModified, OnSiteTemplateModified, OnSiteModified)
		{
		}
	}
	public class SiteRenderEngine<TContent,TSite,TSiteTemplate>:
		ISiteRenderEngine
		where TContent:Content
		where TSite:SF.Common.FrontEndContents.Site
		where TSiteTemplate:SiteTemplate
	{
		ConcurrentDictionary<string, long> SiteBinds = new ConcurrentDictionary<string, long>();
		ConcurrentDictionary<long, Site> Sites = new ConcurrentDictionary<long, Site>();
		ConcurrentDictionary<long, IContent> Contents= new ConcurrentDictionary<long, IContent>();

		public ILogger Logger { get; }
		public SiteRenderEngine(
			ILogService LogService,
			IEventSubscriber<EntityChanged< TContent>> OnContentModified,
			IEventSubscriber<EntityChanged<TSite>> OnSiteTemplateModified,
			IEventSubscriber<EntityChanged<TSiteTemplate>> OnSiteModified
			)
		{
			this.Logger = LogService.GetLogger("UI管理器");

			//if (MQProvider != null)
			//{
			//	MsgChannel = MQProvider.OpenChannel().Result;
			//	MsgChannel.EnsureQueue(MsgQueueName, false, false, false, null).Wait();
			//	MsgListenDisposable = MsgChannel.Listen(MsgQueueName, this);
			//}

			OnContentModified.Wait(e =>
			{
				Contents.TryRemove(EntityIdents.Parse<long>(e.Event.Ident ).Id1, out var c);
				return Task.CompletedTask;
			});

			OnSiteTemplateModified.Wait(e =>
			{
				SiteBinds.TryRemove(EntityIdents.Parse<string>(e.Event.Ident).Id1, out var site);
				return Task.CompletedTask;
			});
			OnSiteModified.Wait(e =>
			{
				Sites.TryRemove(EntityIdents.Parse<long>(e.Event.Ident).Id1, out var s);
				return Task.CompletedTask;
			});

		}
		public async Task<IContent> GetContent(long contentId, Lazy<IContentLoader> Loader)
		{
			IContent ctn;
			if (!Contents.TryGetValue(contentId, out ctn))
			{
				ctn = await Loader.Value.LoadContent(contentId);
				ctn = Contents.GetOrAdd(contentId, ctn);
			}
			return ctn;
		}

		async Task<long> GetSiteTemplateIdAsync(string site, IUIConfigLoader ConfigLoader)
		{
			long tid;
			if (!SiteBinds.TryGetValue(site, out tid))
			{
				tid = await ConfigLoader.SiteResolver.Value.FindTemplateId(site);
				tid = SiteBinds.GetOrAdd(site, tid);
			}
			return tid;
		}

		async Task<Site> GetSiteAsync(long templateId, IUIConfigLoader ConfigLoader)
		{
			Site s;
			if (!Sites.TryGetValue(templateId, out s))
			{
				var cfg = await ConfigLoader.SiteConfigLoader.Value.LoadConfig(templateId);
				if (cfg == null) return null;
				s = Site.Create(cfg, ConfigLoader);
				if (s == null)
					return null;
				s = Sites.GetOrAdd(templateId, s);
			}
			return s;
		}
		public async Task<IPageRenderContext> CreatePageContext(
			string site, 
			string page, 
			IDictionary<string, object> args,
			NamedServiceResolver<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			)
		{
			var tid = await GetSiteTemplateIdAsync(site, ConfigLoader);
			return await CreatePageContext(tid, page, args, DataProviderResolver, ConfigLoader);
		}
		public async Task<IPageRenderContext> CreatePageContext(
			long templateId, 
			string page, 
			IDictionary<string, object> args,
			NamedServiceResolver<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			)
		{
			var s = await GetSiteAsync(templateId, ConfigLoader);
			Page p = null;
			if (!(s?.Pages?.TryGetValue(page, out p) ?? false))
				return null;
			var rcb = new RenderContextBuilder(Logger, DataProviderResolver);
			return await rcb.BuildPageRenderContext(
				s,
				p, 
				args, 
				this, 
				ConfigLoader.ContentLoader,
				true
				);
		}


		public async Task<ISiteRenderContext> CreateSiteContext(
			string site,
			NamedServiceResolver<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			)
		{
			var tid = await GetSiteTemplateIdAsync(site, ConfigLoader);
			return await CreateSiteContext(tid, DataProviderResolver, ConfigLoader);
		}
		public async Task<ISiteRenderContext> CreateSiteContext(
			long templateId,
			NamedServiceResolver<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			)
		{
			var s = await GetSiteAsync(templateId, ConfigLoader);
			var rcb = new RenderContextBuilder(Logger, DataProviderResolver);
			return await rcb.BuildSiteRenderContext(s,  this, ConfigLoader.ContentLoader);
		}
	
		
	}
}
