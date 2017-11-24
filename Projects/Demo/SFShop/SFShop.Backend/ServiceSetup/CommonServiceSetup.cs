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

using SF.Sys.Hosting;
using SF.Sys.Services;
using SF.Sys.Services.Management;
using System;
using System.Threading.Tasks;

namespace SFShop.ServiceSetup
{
	public static class CommonServiceSetup
	{
		public static IServiceCollection AddCommonServices(this IServiceCollection Services, EnvironmentType EnvType)
		{
			Services.AddTextMessageServices();


			//Services.AddAuthUserServices();
			Services.AddMemberServices();

			Services.AddAdminServices();
			//Services.AddBizAdminServices();
			Services.AddFrontEndServices(EnvType);
			//Services.AddFriendlyFrontEndServices();

			Services.AddDocumentServices();

			Services.InitServices("系统服务", InitServices);
			return Services;
		}
		static async Task InitServices(IServiceProvider sp, IServiceInstanceManager sim, long? ParentId)
		{
			var SysAdminService = await sim.NewAdminService().Ensure(sp, ParentId);
			//var BizAdminService = await sim.NewBizAdminService().Ensure(sp, ParentId);
			var MenuService = await sim.NewMenuService().Ensure(sp, ParentId);
			var MemberService = await sim.NewMemberService().Ensure(sp, ParentId);

		}


	}
}
