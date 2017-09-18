using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Management.FrontEndContents.Friendly;
using SF.Management.FrontEndContents;
using SF.Metadata;
using SF.Services.Settings;

namespace SF.Management.FrontEndContents
{
	
	public class MobileImageLandingPageManager :
		MobileImageLandingPageManager<Content, IContentManager>
	{
		public MobileImageLandingPageManager(ISettingService<FriendlyContentSetting> SettingService, IContentManager ContentManager) : base(SettingService, ContentManager)
		{
		}
	}

	public class MobileImageLandingPageManager<TContent, TContentManager> :
		ImageItemGroupManager<TContent, TContentManager>,
		IMobileImageLandingPageManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		ISettingService<FriendlyContentSetting> SettingService { get; }
		public MobileImageLandingPageManager(
			ISettingService<FriendlyContentSetting> SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override long EntityId => SettingService.Value.MobileLandingPageImagesId;
	}

}
