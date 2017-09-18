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
	
	public class PCHomeSilderManager :
		PCHomeSilderManager<Content, IContentManager>
	{
		public PCHomeSilderManager(ISettingService<FriendlyContentSetting> SettingService, IContentManager ContentManager) : base(SettingService, ContentManager)
		{
		}
	}

	public class PCHomeSilderManager<TContent, TContentManager> :
		ImageItemGroupManager<TContent, TContentManager>,
		IPCHomeSilderManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		ISettingService<FriendlyContentSetting> SettingService { get; }
		public PCHomeSilderManager(
			ISettingService<FriendlyContentSetting> SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override long EntityId => SettingService.Value.PCHomePageSliderId;
	}

}
