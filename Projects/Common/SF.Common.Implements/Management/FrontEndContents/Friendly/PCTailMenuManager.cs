using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Management.FrontEndContents.Friendly;
using SF.Management.FrontEndContents;
using SF.Metadata;

namespace SF.Management.FrontEndContents
{
	
	public class PCTailMenuManager :
		PCTailMenuManager<Content, IContentManager>
	{
		public PCTailMenuManager(IFriendlyContentSettingService SettingService, IContentManager<Content> ContentManager) : base(SettingService, ContentManager)
		{
		}
	}

	public class PCTailMenuManager<TContent, TContentManager> :
		TextGroupTextItemGroupManager<TContent, TContentManager>,
		IPCTailMenuManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		IFriendlyContentSettingService SettingService { get; }
		public PCTailMenuManager(
			IFriendlyContentSettingService SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override long EntityId => SettingService.Setting.PCHomeTailMenuId;
	}
}
