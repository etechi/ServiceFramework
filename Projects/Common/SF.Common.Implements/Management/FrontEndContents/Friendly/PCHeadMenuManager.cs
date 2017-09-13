﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Management.FrontEndContents.Friendly;
using SF.Management.FrontEndContents;
using SF.Metadata;

namespace SF.Management.FrontEndContents
{
	public class PCHeadMenuManager :
		PCHeadMenuManager<Content, IContentManager>
	{
		public PCHeadMenuManager(IFriendlyContentSettingService SettingService, IContentManager<Content> ContentManager) : base(SettingService, ContentManager)
		{
		}
	}
	
	public class PCHeadMenuManager<TContent, TContentManager> :
		ImageTextItemGroupManager<TContent, TContentManager>,
		IPCHeadMenuManager
		where TContent : Content
		where TContentManager : IContentManager<TContent>
	{
		IFriendlyContentSettingService SettingService { get; }
		public PCHeadMenuManager(
			IFriendlyContentSettingService SettingService,
			IContentManager<TContent> ContentManager
			) : base(ContentManager)
		{
			this.SettingService = SettingService;
		}

		protected override long EntityId => SettingService.Setting.PCHeadMenuId;
	}

}