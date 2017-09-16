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
	public class MobileAdManager :
		MobileAdManager<Content, IContentManager>
	{
		public MobileAdManager(ISettingService<FriendlyContentSetting> SettingService, IContentManager ContentManager) : base(SettingService, ContentManager)
		{
		}
	}
	
	public class MobileAdManager<TContent, TContentManager> :
		ImageItemGroupListManager<TContent,TContentManager>,
		IMobileAdManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		ISettingService<FriendlyContentSetting> SettingService { get; }
		public MobileAdManager(
			ISettingService<FriendlyContentSetting> SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override string ContentGroup=> SettingService.Value.MobileAdCategory;
	}

}
