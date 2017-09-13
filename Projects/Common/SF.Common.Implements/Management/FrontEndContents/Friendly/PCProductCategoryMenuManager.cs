using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Management.FrontEndContents.Friendly;
using SF.Management.FrontEndContents;
using SF.Metadata;

namespace SF.Management.FrontEndContents
{
	

	public class PCProductCategoryMenuManager :
		PCProductCategoryMenuManager<Content, IContentManager>
	{
		public PCProductCategoryMenuManager(IFriendlyContentSettingService SettingService, IContentManager ContentManager) : base(SettingService, ContentManager)
		{
		}
	}

	public class PCProductCategoryMenuManager<TContent, TContentManager> :
		ImageTextItemGroupManager<TContent, TContentManager>,
		IPCProductCategoryMenuManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		IFriendlyContentSettingService SettingService { get; }
		public PCProductCategoryMenuManager(
			IFriendlyContentSettingService SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override long EntityId => SettingService.Setting.PCHeadProductCategoryMenuId;
	}

}
