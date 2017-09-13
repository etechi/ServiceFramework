using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Management.FrontEndContents.Friendly;
using SF.Management.FrontEndContents;
using SF.Metadata;

namespace SF.Management.FrontEndContents
{
	
	public class MobileImageLandingPageManager :
		MobileImageLandingPageManager<Content, IContentManager>
	{
		public MobileImageLandingPageManager(IFriendlyContentSettingService SettingService, IContentManager<Content> ContentManager) : base(SettingService, ContentManager)
		{
		}
	}

	public class MobileImageLandingPageManager<TContent, TContentManager> :
		ImageItemGroupManager<TContent, TContentManager>,
		IMobileImageLandingPageManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		IFriendlyContentSettingService SettingService { get; }
		public MobileImageLandingPageManager(
			IFriendlyContentSettingService SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override long EntityId => SettingService.Setting.MobileLandingPageImagesId;
	}

}
