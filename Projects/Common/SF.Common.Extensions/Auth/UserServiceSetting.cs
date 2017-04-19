using SF.System.Auth.Identity.Models;
using SF.System.Auth.Sessions;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;
using System;
using SF.Clients;
using SF.System.Auth.Identity;
using SF.Core.Times;

namespace SF.System.Auth
{
	public class UserServiceSetting
	{
		public IClientService ClientService { get; set; }
		public Lazy<IIdentService> IdentService { get; set; }
		public Lazy<ISessionService> SessionService { get; set; }
		public Lazy<ITimeService> TimeService { get; set; }
	}

}

