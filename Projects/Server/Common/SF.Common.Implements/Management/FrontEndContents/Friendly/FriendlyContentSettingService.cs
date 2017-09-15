using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Management.FrontEndContents.Friendly;
using SF.Management.FrontEndContents;
using SF.Metadata;

namespace SF.Management.FrontEndContents
{

	public class FriendlyContentSettingService : IFriendlyContentSettingService
	{
		public FriendlyContentSetting Setting { get; }
		public FriendlyContentSettingService(FriendlyContentSetting Setting)
		{
			this.Setting = Setting;
		}
		
	}

}
