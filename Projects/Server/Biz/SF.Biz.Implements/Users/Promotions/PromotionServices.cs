
using SF.Core.Hosting;
using SF.Core.ServiceManagement.Management;
using SF.Management.MenuServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public static class PromotionServices
	{
		public static IServiceCollection AddPromotionServices(this IServiceCollection Services, EnvironmentType EnvType)
		{
			Services.AddMemberInvitationService();

			Services.InitServices("促销服务", InitServices);
			return Services;
		}
		static async Task InitServices(IServiceProvider sp,IServiceInstanceManager sim,long? ParentId)
		{
			await sim.NewMemberInvitationServive().Ensure(sp, ParentId);
		}
	}
}
