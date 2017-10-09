using SF.Core;
using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Management.FrontEndContents;
using SF.Management.FrontEndContents.Runtime;
using SF.Management.FrontEndContents.DataModels;
using SF.Core.ServiceManagement.Management;

namespace SF.Core.ServiceManagement
{
	public static class FrontEndServicesDIExtensions
	{
		public static IServiceCollection AddFrontEndServices(this IServiceCollection sc,string TablePrefix=null)
		{
			sc.AddScoped<ISiteResolver>(sp=>sp.Resolve<ISiteManager>());
			sc.AddScoped<ISiteConfigLoader>(sp=>sp.Resolve<ISiteTemplateManager>());
			sc.AddScoped<IContentLoader>(sp=>sp.Resolve<IContentManager>());

			sc.EntityServices(
				"FrontContent",
				"前端内容",
				b => b.Add<ISiteManager, SiteManager>("Site", "站点")
				.Add<ISiteTemplateManager, SiteTemplateManager>("SiteTemplate", "站点模板")
				.Add<IContentManager, ContentManager>("SiteContent", "站点内容")
				);

			sc.AddSingleton<ISiteRenderEngine, SiteRenderEngine>();
			sc.AddScoped<IRenderContextCreator, RenderContextCreator>();
			sc.AddDataModules<
				SF.Management.FrontEndContents.DataModels.Content, 
				SF.Management.FrontEndContents.DataModels.Site, 
				SF.Management.FrontEndContents.DataModels.SiteTemplate
				>(TablePrefix);

			//sc.AddSingleton<IRenderProvider, RazorRender>("razor");
			return sc;
		}
		public static IServiceInstanceInitializer<ISiteManager> NewSiteManager(this IServiceInstanceManager sim)
		{
			return sim.DefaultService<ISiteManager, SiteManager>(new { });
		}
		public static IServiceInstanceInitializer<ISiteTemplateManager> NewSiteTemplateManager(this IServiceInstanceManager sim)
		{
			return sim.DefaultService<ISiteTemplateManager, SiteTemplateManager>(new { });
		}
		public static IServiceInstanceInitializer<IContentManager> NewSiteContentManager(this IServiceInstanceManager sim)
		{
			return sim.DefaultService<IContentManager, ContentManager>(new { });
		}
	}
}
