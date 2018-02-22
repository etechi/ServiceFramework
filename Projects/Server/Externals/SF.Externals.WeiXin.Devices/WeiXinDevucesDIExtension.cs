using SF.Auth.IdentityServices.Externals;
using SF.Externals.WeiXin.Devices;
using SF.Externals.WeiXin.Mp;
using SF.Sys.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Services
{
    public static class WeiXinDevicesDIExtension
    {
        public static IServiceCollection AddWeiXinDeviceServices(this IServiceCollection sc, WeiXinDeviceServiceSetting setting =null)
        {
			sc.AddManagedTransient<IWeiXinDeviceService, WeiXinDeviceService>();
			sc.AddServiceInstanceInitializer(
				sim => 
					sim.Service<IWeiXinDeviceService, WeiXinDeviceService>(
						new
						{
							Setting = setting ?? new WeiXinDeviceServiceSetting()
						}
					)
					);

			return sc;
		}
        
    }
}
