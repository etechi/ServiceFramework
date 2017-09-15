using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Management.FrontEndContents.Friendly;
using SF.Management.FrontEndContents;
using SF.Metadata;

namespace SF.Management.FrontEndContents
{
	public class MobileAdManager :
		MobileAdManager<Content, IContentManager>
	{
		public MobileAdManager(IFriendlyContentSettingService SettingService, IContentManager ContentManager) : base(SettingService, ContentManager)
		{
		}
	}
	
	public class MobileAdManager<TContent, TContentManager> :
		ImageItemGroupListManager<TContent,TContentManager>,
		IMobileAdManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		IFriendlyContentSettingService SettingService { get; }
		public MobileAdManager(
			IFriendlyContentSettingService SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override string ContentGroup=> SettingService.Setting.MobileAdCategory;
	}

}
