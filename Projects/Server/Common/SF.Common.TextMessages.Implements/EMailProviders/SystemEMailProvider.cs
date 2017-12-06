using SF.Sys.TimeServices;
using System;
using System.Threading.Tasks;

namespace SF.Common.TextMessages.EMailProviders
{
	/// <summary>
	/// 默认邮件提供者
	/// </summary>
	public class SystemEMailProvider : IMsgProvider
	{

        SystemEMailSetting Setting { get; }
        ITimeService TimeService { get; }
		public bool IsDisabled
		{
			get
			{
				return Setting.Disabled;
			}
		}

		public SystemEMailProvider(SystemEMailSetting Setting, ITimeService TimeService)
		{
			this.Setting = Setting;
            this.TimeService = TimeService;
        }
       
		public async Task<string> Send(string target, Message message)
		{
			if (Setting.Disabled)
				return "禁止发送";

            var id = Guid.NewGuid().ToString();
            using (var cli = new System.Net.Mail.SmtpClient(Setting.SMTPServerAddress, Setting.SMTPServerPort))
            {
                cli.UseDefaultCredentials = false;
                cli.EnableSsl = true;
                cli.Credentials = new System.Net.NetworkCredential(
                    Setting.User,
                    Setting.Password
                    );
                
                var msg = new System.Net.Mail.MailMessage(Setting.User,target)
                {
                    Subject = message.Title,
                    Body = message.Body,
                };
                msg.Headers.Add("Message-Id", id);
                await cli.SendMailAsync(msg);
            }
            return id;
        }

	}
}
