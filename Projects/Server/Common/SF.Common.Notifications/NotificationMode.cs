using System.ComponentModel.DataAnnotations;

namespace SF.Common.Notifications
{
	public enum NotificationMode : byte
	{
		/// <summary>
		/// 普通通知
		/// </summary>
        Normal,
		/// <summary>
		/// 全体通知
		/// </summary>
		Boardcast
	}

}
