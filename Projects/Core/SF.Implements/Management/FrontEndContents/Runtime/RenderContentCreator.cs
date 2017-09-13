using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF.Management.FrontEndContents.Runtime
{
	public class RenderContextCreator:
		IRenderContextCreator,
		IUIConfigLoader
	{
		public Lazy<ISiteConfigLoader> SiteConfigLoader { get; }
		public Lazy<ISiteResolver> SiteResolver { get; }
		public Lazy<IContentLoader> ContentLoader { get; }
		public ISiteRenderEngine Engine { get; }
		public NamedServiceResolver<IDataProvider> DataProviderResolver { get; }
		public RenderContextCreator(
			Lazy<ISiteConfigLoader> SiteConfigLoader,
			Lazy<ISiteResolver> SiteResolver,
			Lazy<IContentLoader> ContentLoader,
			ISiteRenderEngine engine,
			NamedServiceResolver<IDataProvider> DataProviderResolver
			)
		{
			this.SiteConfigLoader = SiteConfigLoader;
			this.SiteResolver = SiteResolver;
			this.ContentLoader = ContentLoader;
			this.Engine = engine;
			this.DataProviderResolver = DataProviderResolver;
		}


		public Task<IPageRenderContext> CreatePageContext(
			long templateId, 
			string page, 
			IDictionary<string, object> args 
			)
		{
			return Engine.CreatePageContext(templateId, page, args, DataProviderResolver, this);
		}
		public Task<IPageRenderContext> CreatePageContext(
			string site,
			string page, 
			IDictionary<string, object> args
			)
		{
			return Engine.CreatePageContext(site, page, args, DataProviderResolver, this);
		}


		public Task<ISiteRenderContext> CreateSiteContext(
			long templateId
			)
		{
			return Engine.CreateSiteContext(templateId, DataProviderResolver, this);
		}
		public Task<ISiteRenderContext> CreateSiteContext(
			string site
			)
		{
			return Engine.CreateSiteContext(site, DataProviderResolver, this);
		}
		
	}
}
