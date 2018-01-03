using SF.Sys.Auth;
using SF.Sys.Linq;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
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

				var msg = new System.Net.Mail.MailMessage(Setting.User, Argument.Targets.Single())
				{
					Subject = Argument.Title,
					Body = Argument.Content,
				};
				msg.Headers.Add("Message-Id", id);
				await cli.SendMailAsync(msg);
			}
			return id;
		}

		public async Task<IEnumerable<string>> TargetResolve(IEnumerable<long> TargetIds)
		{
			var re = await UserPropertyResolver.Value.GetClaims(TargetIds.First(), new[] { PredefinedClaimTypes.EMail }, null);
			var id = re.FirstOrDefault();
			return id == null ? Enumerable.Empty<string>() : EnumerableEx.From(id.Value);
		}

		public Task<IEnumerable<string>> GroupResolve(IEnumerable<long> GroupIds)
		{
			throw new NotImplementedException();
		}
	}
}
