using SF.Auth.IdentityServices.Externals;
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
    public static class WeiXinMPDIExtension
    {
        public static IServiceCollection AddWeiXinMPServices(this IServiceCollection sc,WeiXinMpSetting setting=null)
        {
			sc.AddTransient<IWeiXinClient, Externals.WeiXin.Mp.Core.WeiXinClient>();
			sc.AddSingleton<IAccessTokenManager, Externals.WeiXin.Mp.Core.AccessTokenManager>();

			sc.AddManagedScoped<IExternalAuthorizationProvider, Externals.WeiXin.Mp.OAuth2.OAuth2Provider>();

			sc.AddSetting(setting);
			sc.InitService(
				"微信服务号",
				(sp,sim) => 
					sim.Service<IExternalAuthorizationProvider, Externals.WeiXin.Mp.OAuth2.OAuth2Provider>(
						new
						{
							OAuthSetting = new Externals.WeiXin.Mp.OAuth2.OAuth2Setting()
						}
					).WithIdent("weixin.mp")
					);

			return sc;
		}
        
    }
}
