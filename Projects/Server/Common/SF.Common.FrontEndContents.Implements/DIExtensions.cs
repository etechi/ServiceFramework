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

using SF.Common.FrontEndContents;
using SF.Common.FrontEndContents.Runtime;
using SF.Sys.Hosting;
using SF.Sys.Services;
using SF.Sys.Services.Management;

namespace SF.Core.ServiceManagement
{
	public static class FrontEndServicesDIExtensions
	{
		public static IServiceCollection AddFrontEndServices(this IServiceCollection sc, EnvironmentType EnvType, string TablePrefix=null)
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
				SF.Common.FrontEndContents.DataModels.Content, 
				SF.Common.FrontEndContents.DataModels.Site, 
				SF.Common.FrontEndContents.DataModels.SiteTemplate
				>(TablePrefix);

			if(EnvType!=EnvironmentType.Utils)
				sc.AddTransient<IDataProvider, ServiceDataProvider>("service");

			//sc.AddSingleton<IRenderProvider, RazorRender>("razor");
			return sc;
		}
		
		public static IServiceInstanceInitializer<ISiteManager> NewSiteManager(this IServiceInstanceManager sim)
		{
			return sim.DefaultService<ISiteManager, SiteManager>(new { })
				.WithMenuItems(
					"系统管理/前端管理"
					);
		}
		public static IServiceInstanceInitializer<ISiteTemplateManager> NewSiteTemplateManager(this IServiceInstanceManager sim)
		{
			return sim.DefaultService<ISiteTemplateManager, SiteTemplateManager>(new { })
				.WithMenuItems(
					"系统管理/前端管理"
					);
		}
		public static IServiceInstanceInitializer<IContentManager> NewSiteContentManager(this IServiceInstanceManager sim)
		{
			return sim.DefaultService<IContentManager, ContentManager>(new { })
				.WithMenuItems(
					"系统管理/前端管理"
					);
		}
	}
}
