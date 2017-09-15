using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Management.FrontEndContents.Friendly;
using SF.Management.FrontEndContents;
using SF.Metadata;

namespace SF.Management.FrontEndContents
{
	
	public class MobileHomeSilderManager :
		MobileHomeSilderManager<Content, IContentManager>
	{
		public MobileHomeSilderManager(IFriendlyContentSettingService SettingService, IContentManager ContentManager) : base(SettingService, ContentManager)
		{
		}
	}

	public class MobileHomeSilderManager<TContent, TContentManager> :
		ImageItemGroupManager<TContent, TContentManager>,
		IMobileHomeSilderManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		IFriendlyContentSettingService SettingService { get; }
		public MobileHomeSilderManager(
			IFriendlyContentSettingService SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override long EntityId => SettingService.Setting.MobileHomePageSliderId;
	}

	
}
