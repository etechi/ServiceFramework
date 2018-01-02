using System.ComponentModel.DataAnnotations;

namespace SF.Common.Notifications.Senders.EMail
{
	//[Setting(Name = "助通短信设置")]
	public class SystemEMailSetting 
	{

		///<title>禁止发送</title>
		/// <summary>
		/// 禁止发送消息。生成消息日志，但不进行实际发送，主要用于测试
		/// </summary>
		public bool Disabled { get; set; }

		///<title>用户</title>
		/// <summary>
		/// 发信用户名
		/// </summary>
        [Required]
        public string User { get; set; }

		///<title>密码</title>
		/// <summary>
		/// 发信用户SMTP密码
		/// </summary>
        [Required]
        public string Password { get; set; }

		/// <summary>
		/// SMTP服务器地址
		/// </summary>
        [Required]
        public string SMTPServerAddress { get; set; } = "smtp.exmail.qq.com";

		/// <summary>
		/// SMTP服务器端口
		/// </summary>
        public int SMTPServerPort { get; set; } = 25;
    }
}
