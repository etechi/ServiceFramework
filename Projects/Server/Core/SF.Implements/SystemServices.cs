
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
			});

			return Services;
		}
	}
}
