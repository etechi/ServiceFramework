using SF.Auth.IdentityServices.Externals;
using SF.Externals.WeiXin.Open.OAuth2;

namespace SF.Sys.Services
{
	public static class WeiXinOpenDIExtension
    {
        public static IServiceCollection AddWeiXinOpenServices(this IServiceCollection sc, OAuth2Setting setting =null)
        {
			sc.AddManagedScoped<IExternalAuthorizationProvider, OAuth2Provider>();
			sc.InitServices("微信开放平台", async (sp, sim, parent) =>
			 {
				 await sim.Service<IExternalAuthorizationProvider, OAuth2Provider>(
						 new
						 {
							 Setting = setting
						 }
					 ).WithIdent("weixin.open").Ensure(sp, parent);
 
			 });
			return sc;
		}
        
    }
}
