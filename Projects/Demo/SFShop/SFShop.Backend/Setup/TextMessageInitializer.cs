
using System;
using System.Threading.Tasks;
using SF.Auth.IdentityServices.Managers;
using SF.Sys.Hosting;
using SF.Sys.Services;
using SF.Sys.Entities;
using SF.Sys.Services.Management;
using SF.Sys.Settings;
using SF.Common.FrontEndContents;
using SF.Common.Notifications.Management;
using SF.Common.Notifications.Models;

namespace SFShop.Setup
{
	public static class TextMessageInitializer
	{

		public static async Task DataInitialize(IServiceProvider ServiceProvider, EnvironmentType EnvType)
		{
			var pm = ServiceProvider.Resolve<INotificationSendPolicyManager>();
			await pm.EnsurePolicy(
				 SF.Auth.IdentityServices.Internals.ConfirmMessageType.注册.ToString(),
				 "注册验证消息",
				 "注册验证消息",
				 null
				 //,
				 //new SF.Common.TextMessages.Models.MessageSendAction
				 //{
				 // ContentTemplate = "您的注册验证码是:{验证码}",
				 // TargetTemplate = "{手机号}",
				 // ProviderId=0
				 //}
				 );



		}

	}
}
