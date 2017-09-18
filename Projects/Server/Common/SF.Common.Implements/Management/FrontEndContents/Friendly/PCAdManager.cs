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
	public class PCAdManager :
		PCAdManager<Content, IContentManager>
	{
		public PCAdManager(ISettingService<FriendlyContentSetting> SettingService, IContentManager ContentManager) : base(SettingService, ContentManager)
		{
		}
	}
	
	public class PCAdManager<TContent, TContentManager> :
		ImageItemGroupListManager<TContent,TContentManager>,
		IPCAdManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		ISettingService<FriendlyContentSetting> SettingService { get; }
		public PCAdManager(
			ISettingService<FriendlyContentSetting> SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override string ContentGroup=> SettingService.Value.PCAdCategory;
	}

}
