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
using System.Text;
using System.Collections;
using System.Linq;
using SF.Sys.Services;
using SF.Sys.Hosting;
using SF.Sys.Auth;

namespace SF.Sys
{
	public static class SetupExtension
	{
		public static IServiceCollection AddSystemServices(
			this IServiceCollection Services, 
			EnvironmentType EnvType
			)
		{
			if (EnvType == EnvironmentType.Utils)
			{
				Services.AddConsoleDefaultFilePathStructure();
				Services.AddLocalClientService();
				Services.AddSingleton<IAccessTokenGenerator, NotImplementedIAccessTokenGenerator>();
				Services.AddSingleton<IAccessTokenValidator, NotImplementedIAccessTokenValidator>();
				Services.AddLocalInvokeContext();
			}


			Services.AddConfiguration();

			Services.AddNewtonsoftJson();
			Services.AddSystemTimeService();
			Services.AddHttpClientService();
			Services.AddTaskServiceManager();
			Services.AddTimerService();
			Services.AddTimedTaskRunnerService();
			//Services.AddDataContext();

			Services.AddDataEntityProviders();
			Services.AddServiceFeatureControl();

			Services.AddDynamicTypeBuilder();
			Services.AddFilePathResolver();
			Services.AddLocalFileCache();

			Services.AddDefaultSecurityServices();
			Services.AddAutoEntityService();
			Services.AddEventServices();

			//Services.AddCallPlans();
			//Services.AddDefaultCallPlanStorage();

			Services.AddManagedService();
			Services.AddManagedServiceAdminServices();

			Services.AddIdentGenerator();

			Services.AddDefaultDeviceDetector();
			Services.AddDefaultMimeResolver();
			Services.AddSystemSettings();
			//Services.AddFrontEndServices(EnvType);

			Services.AddTestServices();
			Services.AddEntityTestServices();
			Services.AddReminderServices();

			Services.AddSystemDrawing();
			
			Services.AddMicrosoftMemoryCacheAsLocalCache();

			Services.AddDataScope();

			return Services;

		}
	}
}
