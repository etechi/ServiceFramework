using SF.Data.Storage;
using SF.Data;
using SF.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Metadata;
using SF.Clients;
using SF.Security;
using SF.Core.Times;
using SF.Auth.Passport.Internals;

namespace SF.Auth.Passport
{
	
	public class PassportServiceSetting
	{
		public Lazy<IAuthSessionProvider> AuthSessionProvider { get; set; }
		public Lazy<IUserSessionStorage> UserSessionStorage { get; set; }
		public Lazy<IClientAccessInfo> AccessInfo { get; set; }
		public Lazy<ITimeService> TimeService { get; set; }
		public Lazy<ISigninProvider> SigninProvider { get; set; }
	}

}
