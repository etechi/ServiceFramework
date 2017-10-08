
using SF.Core.Hosting;
using SF.Core.ServiceManagement.Management;
using SF.Management.MenuServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public static class BizServices
	{
		public static IServiceCollection AddBizServices(this IServiceCollection Services, EnvironmentType EnvType)
		{

			Services.AddProductServices();

			Services.InitServices("业务服务", InitServices);
			return Services;
		}
		static async Task InitServices(IServiceProvider sp, IServiceInstanceManager sim, long? ParentId)
		{
			await sim.NewProductItemManager().Ensure(sp, ParentId);
			await sim.NewProductManager().Ensure(sp, ParentId);
			await sim.NewProductCategoryManager().Ensure(sp, ParentId);
			await sim.NewProductTypeManager().Ensure(sp, ParentId);
		}

	}
}
