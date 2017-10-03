using SF.Core;
using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Management.FrontEndContents.Friendly;
using SF.Management.FrontEndContents;
using SF.Services.Settings;

namespace SF.Core.ServiceManagement
{
	public static class FriendlyFrontEndServicesDIExtensions
	{
		public static IServiceCollection AddFriendlyFrontEndServices(this IServiceCollection sc)
		{

			sc.AddSetting<FriendlyContentSetting>();

			sc.AddManagedScoped<IMobileHomeSilderManager, MobileHomeSilderManager>();
			sc.AddManagedScoped<IMobileImageLandingPageManager, MobileImageLandingPageManager>();
			sc.AddManagedScoped<IMobileProductCategoryMenuManager, MobileProductCategoryMenuManager>();

			sc.AddManagedScoped<IPCHeadMenuManager, PCHeadMenuManager>();
			sc.AddManagedScoped<IPCTailMenuManager, PCTailMenuManager>();
			sc.AddManagedScoped<IPCHomeSilderManager, PCHomeSilderManager>();
			sc.AddManagedScoped<IPCProductCategoryMenuManager, PCProductCategoryMenuManager>();

			sc.AddManagedScoped<IPCAdManager, PCAdManager>();
			sc.AddManagedScoped<IMobileAdManager, MobileAdManager>();


			return sc;
		}
	}
}
