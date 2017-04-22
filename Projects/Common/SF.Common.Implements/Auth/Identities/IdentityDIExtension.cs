using SF.Auth.Identities.Internals;
using SF.Core.DI;
using SF.Core.ManagedServices.Admin;

namespace SF.Auth.Identities
{
	public static class IdentityDIExtension
	{
		public static IDIServiceCollection UseIdentity(
			this IDIServiceCollection sc
			)
		{
			sc.AddScoped<IIdentityService, IdentityService>();
			return sc;
		}
	}
}