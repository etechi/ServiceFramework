using System.ComponentModel.DataAnnotations;

namespace SF.Common.Notifications
{
	public enum NotificationMode : byte
    {
        [Display(Name ="普通通知")]
        Normal,
        [Display(Name = "全体通知")]
        Boardcast
    }

}
