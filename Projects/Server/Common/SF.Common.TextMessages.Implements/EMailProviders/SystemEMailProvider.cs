using SF.Sys.Auth;
using SF.Sys.TimeServices;
using System;
using System.Linq;
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
			var re=await UserPropertyResolver.Value.GetClaims(TargetId, new[] { "email" },null);
			return re.FirstOrDefault()?.Value;

		}

		public async Task<string> Send(MsgSendArgument Argument)
		{
			if (Setting.Disabled)
				throw new ArgumentException("禁止发送");

			var id = Guid.NewGuid().ToString();
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
