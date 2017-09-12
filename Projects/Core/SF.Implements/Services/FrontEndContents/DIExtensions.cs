using SF.Core;
using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Services.FrontEndContents;
using SF.Services.FrontEndContents.Runtime;

namespace SF.Core.ServiceManagement
{
	public static class FrontEndServicesDIExtensions
	{
		public static IServiceCollection AddFrontEndServices(this IServiceCollection sc)
		{
			sc.AddScoped<ISiteResolver, SiteManager>();
			sc.AddScoped<ISiteConfigLoader, SiteTemplateManager>();
			sc.AddScoped<IContentLoader, ContentManager>();


			sc.AddScoped<ISiteManager, SiteManager>();
			sc.AddScoped<ISiteTemplateManager, SiteTemplateManager>();
			sc.AddScoped<IContentManager, ContentManager>();



			sc.AddSingleton<ISiteRenderEngine, SiteRenderEngine>();
			sc.AddScoped<IRenderContextCreator, RenderContextCreator>();


			//sc.AddSingleton<IRenderProvider, RazorRender>("razor");
			return sc;
		}
	}
}
