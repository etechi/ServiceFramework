using SF.Auth.Identity.Internals;
using SF.Core.DI;
using SF.Core.ManagedServices.Admin;

namespace SF.Auth.Identity
{
	public static class IdentityDIExtension
	{
		public static IDIServiceCollection UseIdentity(
			this IDIServiceCollection sc
			)
		{
			sc.AddScoped<IIdentService, IdentService>();
			return sc;
		}
	}
}