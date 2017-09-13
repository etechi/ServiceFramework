using SF.Core;
using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Management.FrontEndContents.Friendly;

namespace SF.Core.ServiceManagement
{
	public static class FriendlyFrontEndServicesDIExtensions
	{
		public static IServiceCollection AddFriendlyFrontEndServices(this IServiceCollection sc)
		{
			sc.AddScopedManaged<IPCHea
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
