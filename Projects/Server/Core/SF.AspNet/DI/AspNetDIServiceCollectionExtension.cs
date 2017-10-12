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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using System.Web.Http;
using SF.AspNet.Formatting;
using SF.Core.Logging;

namespace SF.Core.ServiceManagement
{

	public static class IDIRegisterExtension
	{
		public static void RegisterMvcControllers(this IServiceCollection sc, params System.Reflection.Assembly[] Assemblies)
		{
			foreach (var ct in from ass in Assemblies
							   from type in ass.GetTypes()
							   where type.IsVisible && type.GetInterface(typeof(IController).FullName) != null
							   select type
							   )
			{
				sc.AddTransient(ct);
			}
		}



		public static void InitWebApiFormatter(this IServiceProvider sp, HttpConfiguration cfg)
		{
			cfg.Formatters.Remove(cfg.Formatters.XmlFormatter);
			cfg.Formatters.Remove(cfg.Formatters.JsonFormatter);
			cfg.Formatters.Add(
				new SF.AspNet.Formatting.JsonSerializerFormatter(
					sp.Resolve<SF.Core.Serialization.IJsonSerializer>()
					)
				);
		}
		public static void InitMvcValueProvider(this IServiceProvider sp)
		{
			ValueProviderFactories.Factories
						.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());

			ValueProviderFactories.Factories.Add(
				new TypeSerializerValueProviderFactory(
					sp.Resolve<Serialization.IJsonSerializer>()
					)
				);
		}

		public static void RegisterWebApiControllers(this IServiceCollection sc, HttpConfiguration cfg)
		{
			var ass_resolver = cfg.Services.GetAssembliesResolver();
			var type_resolver = cfg.Services.GetHttpControllerTypeResolver();
			foreach (var type in type_resolver.GetControllerTypes(ass_resolver))
			{
				System.Diagnostics.Debugger.Log(1, "DI", $"Register WebApiController {type.FullName}\n");
				sc.AddSingleton(type);
			}
		}

		public static void ReplaceDependenceResolver(this IServiceProvider ServiceProvider, HttpConfiguration configuration)
		{
			configuration.DependencyResolver = new AspNet.DI.WebApiDependenceResolver(ServiceProvider);
			System.Web.Mvc.DependencyResolver.SetResolver(
				new SF.AspNet.DI.MvcDependenceResolver()
				);
			AspNet.DI.DIHttpModule.ServiceProvider=ServiceProvider;
		}

		public static IServiceCollection UseAspNetDIScopeLogger(this IServiceCollection sc)
		{
			//sc.Add(
			//	typeof(SF.Core.Logging.IDIScopeLogger<>), 
			//	typeof(SF.Core.Logging.AspNetDIScopeLogger<>), 
			//	ServiceLifetime.Scoped
			//	);
			return sc;
		}
		public static IServiceCollection UseAspNetFilePathStructure(this IServiceCollection sc)
		{
			sc.AddSingleton<Hosting.IDefaultFilePathStructure>(sp =>
				new AspNet.DefaultFilePathStructure()
				);
			return sc;
		}
	}
}
