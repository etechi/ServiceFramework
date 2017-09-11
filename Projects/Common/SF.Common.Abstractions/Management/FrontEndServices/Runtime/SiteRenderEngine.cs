using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
namespace SF.Management.FrontEndServices
{
	public class SiteLoadContext
	{
		public List<string> Messages { get; } = new List<string>();
	}
	public class SiteRenderEngine:
		ISiteRenderEngine,
		MQ.IMsgListener,
		IDisposable
	{
		ConcurrentDictionary<string, int> SiteBinds = new ConcurrentDictionary<string, int>();
		ConcurrentDictionary<int, Site> Sites = new ConcurrentDictionary<int, Site>();
		ConcurrentDictionary<int, IContent> Contents= new ConcurrentDictionary<int, IContent>();

		MQ.IMsgChannel MsgChannel { get; }
		IDisposable MsgListenDisposable { get; }
		public Logging.ILogger Logger { get; }
		public readonly string MsgQueueName="ServiceProtocol.Biz.UIManager";
		public SiteRenderEngine(
			MQ.IMsgQueueProvider MQProvider,
			Logging.ILogService LogService
			)
		{
			this.Logger = LogService.GetLogger("UI管理器");

			if (MQProvider != null)
			{
				MsgChannel = MQProvider.OpenChannel().Result;
				MsgChannel.EnsureQueue(MsgQueueName, false, false, false, null).Wait();
				MsgListenDisposable = MsgChannel.Listen(MsgQueueName, this);
			}
		}
		public async Task<IContent> GetContent(int contentId, IContentLoader Loader)
		{
			IContent ctn;
			if (!Contents.TryGetValue(contentId, out ctn))
			{
				ctn = await Loader.LoadContent(contentId);
				ctn = Contents.GetOrAdd(contentId, ctn);
			}
			return ctn;
		}

		async Task<int> GetSiteTemplateIdAsync(string site, IUIConfigLoader ConfigLoader)
		{
			int tid;
			if (!SiteBinds.TryGetValue(site, out tid))
			{
				tid = await ConfigLoader.SiteResolver.FindTemplateId(site);
				tid = SiteBinds.GetOrAdd(site, tid);
			}
			return tid;
		}

		async Task<Site> GetSiteAsync(int templateId, IUIConfigLoader ConfigLoader)
		{
			Site s;
			if (!Sites.TryGetValue(templateId, out s))
			{
				var cfg = await ConfigLoader.SiteConfigLoader.LoadConfig(templateId);
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
			DI.IDIProviderResolver<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			)
		{
			var tid = await GetSiteTemplateIdAsync(site, ConfigLoader);
			return await CreatePageContext(tid, page, args, DataProviderResolver, ConfigLoader);
		}
		public async Task<IPageRenderContext> CreatePageContext(
			int templateId, 
			string page, 
			IDictionary<string, object> args,
			DI.IDIProviderResolver<IDataProvider> DataProviderResolver,
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
			DI.IDIProviderResolver<IDataProvider> DataProviderResolver,
			IUIConfigLoader ConfigLoader
			)
		{
			var tid = await GetSiteTemplateIdAsync(site, ConfigLoader);
			return await CreateSiteContext(tid, DataProviderResolver, ConfigLoader);
		}
		public async Task<ISiteRenderContext> CreateSiteContext(
			int templateId,
			DI.IDIProviderResolver<IDataProvider> DataProviderResolver, 
			IUIConfigLoader ConfigLoader
			)
		{
			var s = await GetSiteAsync(templateId, ConfigLoader);
			var rcb = new RenderContextBuilder(Logger, DataProviderResolver);
			return await rcb.BuildSiteRenderContext(s,  this, ConfigLoader.ContentLoader);
		}
		public void Dispose()
		{
			MsgListenDisposable.Dispose();
			MsgChannel.Dispose();
		}

		void IMsgListener.OnBinded()
		{
		}

		void IMsgListener.OnError()
		{
		}
		void RemoveSiteBind(string site)
		{
			int tid;
			SiteBinds.TryRemove(site, out tid);
		}
		void RemoveSiteTemplate(int templateId)
		{
			Site s;
			Sites.TryRemove(templateId, out s);
		}
		void RemoveContent(int contentId)
		{
			IContent c;
			Contents.TryRemove(contentId, out c);
		}
		void IMsgListener.OnMessageArrived(IInputMessage message)
		{
			var type = message.Header("type") as string;
			var value = message.Header("target") as string;
			switch (type)
			{
				case "SiteBindChanged":
					{
						RemoveSiteBind(value);
						message.Acknowledge();
						break;
					}
				case "SiteTemplateChanged":
					{
						int tid;
						if (!int.TryParse(value, out tid))
						{
							message.Reject();
							return;
						}
						RemoveSiteTemplate(tid);
						break;
					}
				case "ContentChanged":
					{
						int tid;
						if (!int.TryParse(value, out tid))
						{
							message.Reject();
							return;
						}
						RemoveContent(tid);
						break;
					}
				default:
					message.Reject();
					break;
			}
		}

		void IMsgListener.OnShutdown(string reason)
		{
		}

		void IMsgListener.OnUnbinded()
		{
		}
		public void NotifySiteBindChanged(string site)
		{

			if (MsgChannel == null)
			{
				RemoveSiteBind(site);
				return;
			}
			MsgChannel.Writer.Write(new OutputMessage
			{
				RoutingKey = MsgQueueName,
				Headers = new Dictionary<string, object>
				{
					{"type","SiteBindChanged" },
					{"target",site }
				}
			});
		}
		public void NotifySiteTemplateChanged(int templateId)
		{
			if (MsgChannel == null)
			{
				RemoveSiteTemplate(templateId);
				return;
			}
			MsgChannel.Writer.Write(new OutputMessage
			{
				RoutingKey = MsgQueueName,
				Headers = new Dictionary<string, object>
				{
					{"type","SiteTemplateChanged" },
					{"target",templateId.ToString() }
				}
			});
		}
		public void NotifyContentChanged(int contentId)
		{
			if (MsgChannel == null)
			{
				RemoveContent(contentId);
				return;
			}
			MsgChannel.Writer.Write(new OutputMessage
			{
				RoutingKey = MsgQueueName,
				Headers = new Dictionary<string, object>
				{
					{"type","ContentChanged" },
					{"target",contentId.ToString() }
				}
			});
		}
	}
}
