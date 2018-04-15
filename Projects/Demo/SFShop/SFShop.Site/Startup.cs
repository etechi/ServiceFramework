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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SF.Core.ServiceManagement;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SF.Sys.AspNetCore;
using SF.Sys.Hosting;
using SF.Sys.Services;
using SF.Auth.IdentityServices;
using SF.Sys.Data;
using System.Threading.Tasks;
using SF.Sys.ServiceFeatures;

namespace SFShop
{
	public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
		{
			services.AddSingleton(sp => new DataSourceConfig { ConnectionString = Configuration["ConnectionStrings:Default"] });

			services.AddAspNetCoreSystemServices();
			services.AddMvc()
				.AddJsonOptions(
					s => SF.Sys.Serialization.Newtonsoft.JsonSerializer.ApplySetting(
						s.SerializerSettings,
						new SF.Sys.Serialization.JsonSetting
						{
							IgnoreDefaultValue = true,
							WithType = false
						})
				);

			var ins = AppBuilder.Init(SF.Sys.Hosting.EnvironmentType.Production, services)
				.With((SF.Sys.Services.IServiceCollection sc) =>
					sc.AddAspNetCoreSupport()
					//.AddAccessTokenHandler(
					//	"HYGOU",
					//	"123456",
					//	null,
					//	null
					//	)
				)
				.OnEnvType(
					t=>t!=EnvironmentType.Utils,
					sc=>
					{
						sc.AddFrontSideMvcContentRenderProvider();
						sc.AddNetworkService();
						sc.AddAspNetCoreServiceInterface();
						sc.AddJwtAuthSupports(new JwtAuthSettings
						{
							SecurityKey = "1234567890123456",
							Issuer = "issuer",
							Audience = "audience"
						});
					}
					)
				.Build();
			return ins.ServiceProvider;
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			//StartServices(app, applicationLifetime);

			app.ApplicationCommonConfigure(env);
			var mvc=app.UseMvc(routes =>
			{
				routes.MapRoute(
				  name: "Mobile",
				  template: "m/{*id}",
				  defaults: new { controller = "Mobile", action = "Index"}
				);

				routes.MapRoute(
				  name: "指定产品",
				  template: "item/{ItemId}",
				  defaults: new { controller = "Product", action = "Item" }
				);
				routes.MapRoute(
				 name: "活动",
				 template: "activity/{action}/",
				 defaults: new { controller = "ActivityPC" }
			   );
				routes.MapRoute(
				   name: "最新轮次的产品",
				   template: "item/{ItemId}",
				   defaults: new { controller = "Product", action = "Item" }
				 );
				routes.MapRoute(
				   name: "产品",
				   template: "product/{ProductId}",
				   defaults: new { controller = "Product", action = "Product" }
				 );
				routes.MapRoute(
				   name: "指定分类的产品列表",
				   template: "cat/{CategoryId?}",
				   defaults: new { controller = "Product", action = "Category"}
				 );
				routes.MapRoute(
				   name: "产品集产品列表",
				   template: "col/{CategoryId}",
				   defaults: new { controller = "Product", action = "Collection" }
				 );
				routes.MapRoute(
				   name: "产品转籍",
				   template: "special/{Id}",
				   defaults: new { controller = "Product", action = "Special" }
				 );
				routes.MapRoute(
				   name: "搜索",
				   template: "search",
				   defaults: new { controller = "Product", action = "Search" }
				 );
				routes.MapRoute(
					name: "购物车",
					template: "cart",
					defaults: new { controller = "ShoppingCart", action = "Index" }
				  );
				routes.MapRoute(
					name: "揭晓",
					template: "open",
					defaults: new { controller = "Round", action = "List" }
				  );
				routes.MapRoute(
					name: "晒单",
					template: "shared/{id}",
					defaults: new { controller = "Round", action = "Shared" }
				  );
				routes.MapRoute(
					name: "晒单列表",
					template: "shared",
					defaults: new { controller = "Round", action = "Shared" }
				  );
				routes.MapRoute(
					name: "其他人",
					template: "other/{id}",
					defaults: new { controller = "Other", action = "Index" }
				  );

				routes.MapRoute(
					name: "admin-signin",
					template: "admin/signin",
					defaults: new { controller = "admin",action="signin" }
					);
				routes.MapRoute(
					name: "admin",
					template: "admin/{*id}",
					defaults:new { controller = "admin",action="index" }
					);

				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
			
		}

		private static void StartServices(IApplicationBuilder app, IApplicationLifetime applicationLifetime)
		{
			var ins = app.ApplicationServices.Resolve<IAppInstance>();
			if (ins.EnvType != EnvironmentType.Utils)
			{
				var disposable = Task.Run(() =>
					  app.ApplicationServices.BootServices()
					).Result;
				applicationLifetime.ApplicationStopping.Register(() =>
				{
					disposable.Dispose();
				});
			}
		}
	}
}
