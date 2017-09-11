using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
using ServiceProtocol.DI;

namespace ServiceProtocol.Biz.UIManager.Runtime
{
	public class RenderContextCreator:
		IRenderContextCreator,
		IUIConfigLoader
	{
		public ISiteConfigLoader SiteConfigLoader { get; }
		public ISiteResolver SiteResolver { get; }
		public IContentLoader ContentLoader { get; }
		public ISiteRenderEngine Engine { get; }
		public DI.IDIProviderResolver<IDataProvider> DataProviderResolver { get; }
		public RenderContextCreator(
			ISiteConfigLoader SiteConfigLoader,
			ISiteResolver SiteResolver,
			IContentLoader ContentLoader,
			ISiteRenderEngine engine,
			DI.IDIProviderResolver<IDataProvider> DataProviderResolver
			)
		{
			this.SiteConfigLoader = SiteConfigLoader;
			this.SiteResolver = SiteResolver;
			this.ContentLoader = ContentLoader;
			this.Engine = engine;
			this.DataProviderResolver = DataProviderResolver;
		}


		public Task<IPageRenderContext> CreatePageContext(
			int templateId, 
			string page, 
			IDictionary<string, object> args, 
			IDIScope scope
			)
		{
			return Engine.CreatePageContext(templateId, page, args, DataProviderResolver, this);
		}
		public Task<IPageRenderContext> CreatePageContext(
			string site,
			string page, 
			IDictionary<string, object> args, 
			IDIScope scope
			)
		{
			return Engine.CreatePageContext(site, page, args, DataProviderResolver, this);
		}


		public Task<ISiteRenderContext> CreateSiteContext(
			int templateId,
			IDIScope scope
			)
		{
			return Engine.CreateSiteContext(templateId, DataProviderResolver, this);
		}
		public Task<ISiteRenderContext> CreateSiteContext(
			string site,
			IDIScope scope
			)
		{
			return Engine.CreateSiteContext(site, DataProviderResolver, this);
		}
		
	}
}
