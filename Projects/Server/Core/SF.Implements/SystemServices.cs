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


using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public static class SystemServices
	{
		public static IServiceCollection AddSystemServices(this IServiceCollection Services, EnvironmentType EnvType)
		{
			if (EnvType == EnvironmentType.Utils)
				Services.AddConsoleDefaultFilePathStructure();

			Services.AddNewtonsoftJson();
			Services.AddSystemTimeService();

			Services.AddTaskServiceManager();

			//Services.AddDataContext();

			Services.AddDataEntityProviders();
			Services.AddServiceFeatureControl();

			Services.AddDynamicTypeBuilder();
			Services.AddFilePathResolver();
			Services.AddLocalFileCache();

			Services.AddDefaultSecurityServices();
			Services.AddAutoEntityService();
			Services.AddEventServices();

			Services.AddCallPlans();
			Services.AddDefaultCallPlanStorage();

			Services.AddManagedService();
			Services.AddManagedServiceAdminServices();

			Services.AddIdentGenerator();

			Services.AddDefaultKBServices();
			Services.AddSystemSettings();
			Services.AddFrontEndServices();

			Services.AddTestServices();
			Services.AddEntityTestServices();



			Services.InitServices("系统服务", async (sp, sim, ParentId) =>
			{
				await sim.NewDataProtectorService().Ensure(sp, ParentId);
				await sim.NewPasswordHasherService().Ensure(sp, ParentId);

				await sim.NewSiteManager().Ensure(sp, ParentId);
				await sim.NewSiteTemplateManager().Ensure(sp, ParentId);
				await sim.NewSiteContentManager().Ensure(sp, ParentId);
			});

			return Services;
		}
	}
}
