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
namespace SF.Core.ServiceManagement
{
	public static class FrontEndServicesDIExtensions
	{
		public static IServiceCollection AddFrontEndServices(this IServiceCollection sc,string TablePrefix=null)
		{
			sc.AddScoped<ISiteResolver, SiteManager>();
			sc.AddScoped<ISiteConfigLoader, SiteTemplateManager>();
			sc.AddScoped<IContentLoader, ContentManager>();


			sc.AddScoped<ISiteManager, SiteManager>();
			sc.AddScoped<ISiteTemplateManager, SiteTemplateManager>();
			sc.AddScoped<IContentManager, ContentManager>();



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
	}
}
