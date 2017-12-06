using System.ComponentModel.DataAnnotations;

namespace SF.Common.TextMessages.EMailProviders
{
	//[Setting(Name = "助通短信设置")]
	public class SystemEMailSetting 
	{

		/// <summary>
		/// 是否禁用
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
		/// </summary
        public int SMTPServerPort { get; set; } = 25;
    }
}
