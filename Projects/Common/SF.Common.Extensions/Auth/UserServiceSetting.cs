using SF.Auth.Identity.Models;
using SF.Auth.Sessions;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;
using System;
using SF.Clients;
using SF.Auth.Identity;
using SF.Core.Times;

namespace SF.Auth
{
	public class UserServiceSetting
	{
		public IIdentService IdentService { get; set; }
		public Lazy<ITimeService> TimeService { get; set; }
	}

}

