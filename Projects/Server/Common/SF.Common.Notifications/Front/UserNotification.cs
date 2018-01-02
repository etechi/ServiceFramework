using SF.Sys.Annotations;
using SF.Sys.Auth;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Notifications.Front
{

	public class UserNotification
    {
		/// <summary>
		/// ID
		/// </summary>
        [Key]
        public long Id { get; set; }
		
		/// <summary>
		/// 通知模式
		/// </summary>
		public NotificationMode Mode { get; set; }
		///<title>通知标题</title>
		/// <summary>
		/// 一句话通知标题，普通通知仅有标题
		/// </summary>
		[MaxLength(100)]
        [Required]
        public string Name { get; set; }

		///<title>通知链接</title>
		/// <summary>
		/// 用户可以通过该链接跳转至指定页面
		/// </summary>
		[MaxLength(100)]
        public string Link { get; set; }

		///<title>通知图片</title>
		/// <summary>
		/// 一般只有促销类的通知才会有图片
		/// </summary>
        [Image]
        public string Image { get; set; }

		///<title>有通知内容</title>
		/// <summary>
		/// 是否有通知内容，一般只有促销类的通知才会有详细内容
		/// </summary>
        public bool HasBody { get; set; }

		/// <summary>
		/// 通知发出的时间
		/// </summary>
        public DateTime Time { get; set; }

		/// <summary>
		/// 通知发送用户
		/// </summary>
		[EntityIdent(typeof(User), nameof(SenderName))]
        public long? SenderId { get; set; }

		/// <summary>
		/// 通知发送用户
		/// </summary>
        public string SenderName { get; set; }

		///<title>通知阅读时间</title>
		/// <summary>
		/// 当用户在通知列表停留一段时间，或进入通知详细页时，通知被标记为已读状态
		/// </summary>
        public DateTime? ReadTime { get; set; }

		/// <summary>
		/// 业务跟踪参数
		/// </summary>
        public string BizTrackId { get; set; }
        
		/// <summary>
		/// 详细内容
		/// </summary>
		public string Content { get; set; }
    }

}
