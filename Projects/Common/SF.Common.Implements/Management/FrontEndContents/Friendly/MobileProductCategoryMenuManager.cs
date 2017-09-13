using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Management.FrontEndContents.Friendly;
using SF.Management.FrontEndContents;
using SF.Metadata;

namespace SF.Management.FrontEndContents
{
	
	public class MobileProductCategoryMenuManager :
		MobileProductCategoryMenuManager<Content, IContentManager>
	{
		public MobileProductCategoryMenuManager(IFriendlyContentSettingService SettingService, IContentManager ContentManager) : base(SettingService, ContentManager)
		{
		}
	}

	public class MobileProductCategoryMenuManager<TContent, TContentManager> :
		ImageTextItemGroupManager<TContent, TContentManager>,
		IMobileProductCategoryMenuManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		IFriendlyContentSettingService SettingService { get; }
		public MobileProductCategoryMenuManager(
			IFriendlyContentSettingService SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override long EntityId => SettingService.Setting.MobileProductCategoryMenuId;
	}

}
