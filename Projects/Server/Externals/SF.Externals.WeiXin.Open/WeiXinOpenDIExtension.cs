using SF.Auth.IdentityServices.Externals;
using SF.Externals.WeiXin.Open.OAuth2;

namespace SF.Sys.Services
{
	public static class WeiXinOpenDIExtension
    {
        public static IServiceCollection AddWeiXinOpenServices(this IServiceCollection sc, OAuth2Setting setting =null)
        {
			sc.AddManagedScoped<IOAuthAuthorizationProvider, OAuth2Provider>();

			sc.AddServiceInstanceInitializer(
				sim => 
					sim.Service<IOAuthAuthorizationProvider, OAuth2Provider>(
						setting
					).WithIdent("weixin.open")
					);

			return sc;
		}
        
    }
}
