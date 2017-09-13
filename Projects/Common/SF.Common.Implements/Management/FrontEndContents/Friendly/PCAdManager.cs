using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Management.FrontEndContents.Friendly;
using SF.Management.FrontEndContents;
using SF.Metadata;

namespace SF.Management.FrontEndContents
{
	public class PCAdManager :
		PCAdManager<Content, IContentManager>
	{
		public PCAdManager(IFriendlyContentSettingService SettingService, IContentManager ContentManager) : base(SettingService, ContentManager)
		{
		}
	}
	
	public class PCAdManager<TContent, TContentManager> :
		ImageItemGroupListManager<TContent,TContentManager>,
		IPCAdManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		IFriendlyContentSettingService SettingService { get; }
		public PCAdManager(
			IFriendlyContentSettingService SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override string ContentGroup=> SettingService.Setting.PCAdCategory;
	}

}
