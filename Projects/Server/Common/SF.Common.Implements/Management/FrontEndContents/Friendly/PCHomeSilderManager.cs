using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Management.FrontEndContents.Friendly;
using SF.Management.FrontEndContents;
using SF.Metadata;

namespace SF.Management.FrontEndContents
{
	
	public class PCHomeSilderManager :
		PCHomeSilderManager<Content, IContentManager>
	{
		public PCHomeSilderManager(IFriendlyContentSettingService SettingService, IContentManager ContentManager) : base(SettingService, ContentManager)
		{
		}
	}

	public class PCHomeSilderManager<TContent, TContentManager> :
		ImageItemGroupManager<TContent, TContentManager>,
		IPCHomeSilderManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		IFriendlyContentSettingService SettingService { get; }
		public PCHomeSilderManager(
			IFriendlyContentSettingService SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override long EntityId => SettingService.Setting.PCHomePageSliderId;
	}

}
