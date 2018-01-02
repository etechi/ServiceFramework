using SF.Sys.Auth;
using SF.Sys.TimeServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Common.Notifications.Senders.EMail
{
	/// <summary>
	/// 默认邮件提供者
	/// </summary>
	public class SystemEMailProvider : INotificationSendProvider
	{

        SystemEMailSetting Setting { get; }
        ITimeService TimeService { get; }
		Lazy<IUserProfileService> UserPropertyResolver { get; }
		public bool IsDisabled
		{
			get
			{
				return Setting.Disabled;
			}
		}
		public SystemEMailProvider(
			SystemEMailSetting Setting, 
			ITimeService TimeService,
			Lazy<IUserProfileService> UserPropertyResolver
			)
		{
			this.Setting = Setting;
            this.TimeService = TimeService;
			this.UserPropertyResolver= UserPropertyResolver;

		}

		public async Task<string> TargetResolve(long TargetId)
		{
			var re=await UserPropertyResolver.Value.GetClaims(TargetId, new[] { PredefinedClaimTypes.EMail },null);
			return re.FirstOrDefault()?.Value;

		}

		public async Task<string> Send(ISendArgument Argument)
		{
			if (Setting.Disabled)
				return "禁止发送";

			var id = "SFMN" + Argument.Id;
			using (var cli = new System.Net.Mail.SmtpClient(Setting.SMTPServerAddress, Setting.SMTPServerPort))
			{
				cli.UseDefaultCredentials = false;
				cli.EnableSsl = true;
				cli.Credentials = new System.Net.NetworkCredential(
					Setting.User,
					Setting.Password
					);

				var msg = new System.Net.Mail.MailMessage(Setting.User, Argument.Target)
				{
					Subject = Argument.Title,
					Body = Argument.Content,
				};
				msg.Headers.Add("Message-Id", id);
				await cli.SendMailAsync(msg);
			}
			return id;
		}
	}
}
