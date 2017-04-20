using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Identity.Models;
using SF.Auth.Identity.Internals;
using SF.Clients;
using SF.Security;
using SF.Core.Times;
using SF.Core.Caching;

namespace SF.Auth.Identity
{

	public class IdentServiceSetting
	{
		public bool VerifyCodeVisible { get; set; }

		public Lazy<ILocalCache<VerifyCode>> VerifyCodeCache { get; set; }
		public Lazy<IIdentStorage> IdentStorage { get; set; }
		public Lazy<IClientService> ClientService { get; set; }
		public Lazy<IDataProtector> DataProtector { get; set; }
		public Lazy<IPasswordHasher> PasswordHasher { get; set; }

		public Lazy<ITimeService> TimeService { get; set; }
		public Lazy<ILocalCache<AccessTokenPassword>> AccessTokenPasswordCache { get; set; }

		public Lazy<IIdentBindProvider> SignupIdentProvider { get; set; }
		public Lazy<IIdentBindProvider[]> SigninIdentProviders { get; set; }

	
		public Lazy<IIdentGenerator> IdentGenerator { get; set; }
	}

}
