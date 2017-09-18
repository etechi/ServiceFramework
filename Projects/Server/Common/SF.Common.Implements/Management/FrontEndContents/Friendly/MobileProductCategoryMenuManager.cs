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
	
	public class MobileProductCategoryMenuManager :
		MobileProductCategoryMenuManager<Content, IContentManager>
	{
		public MobileProductCategoryMenuManager(ISettingService<FriendlyContentSetting> SettingService, IContentManager ContentManager) : base(SettingService, ContentManager)
		{
		}
	}

	public class MobileProductCategoryMenuManager<TContent, TContentManager> :
		ImageTextItemGroupManager<TContent, TContentManager>,
		IMobileProductCategoryMenuManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		ISettingService<FriendlyContentSetting> SettingService { get; }
		public MobileProductCategoryMenuManager(
			ISettingService<FriendlyContentSetting> SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override long EntityId => SettingService.Value.MobileProductCategoryMenuId;
	}

}
