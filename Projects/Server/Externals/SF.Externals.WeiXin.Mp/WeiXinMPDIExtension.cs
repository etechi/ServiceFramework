using SF.Auth.IdentityServices.Externals;
using SF.Common.Notifications.Senders;
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
			sc.AddManagedScoped<INotificationSendProvider, Externals.WeiXin.Mp.TextMessages.MsgProvider>();
			sc.AddManagedScoped<INotificationSendProvider, Externals.WeiXin.Mp.InstantMessages.MessageSender>();

			sc.AddSetting(setting);
			sc.InitServices(
				"微信服务号",
				async (sp,sim, pid) =>
				{
					await sim.Service<IExternalAuthorizationProvider, Externals.WeiXin.Mp.OAuth2.OAuth2Provider>(
						new
						{
							OAuthSetting = new Externals.WeiXin.Mp.OAuth2.OAuth2Setting()
						}
					).WithIdent("weixin.mp").Ensure(sp,pid);
					await sim.Service<INotificationSendProvider, Externals.WeiXin.Mp.TextMessages.MsgProvider>(
						new
						{
							Setting = new Externals.WeiXin.Mp.TextMessages.TemplateMessageSetting
							{
								Disabled=false
							}
						}
						).WithIdent("weixin.template").Ensure(sp, pid);
					await sim.Service<INotificationSendProvider, Externals.WeiXin.Mp.InstantMessages.MessageSender>(null)
						.WithIdent("weixin.message")
						.Ensure(sp, pid);

				}
					);

			return sc;
		}
        
    }
}
