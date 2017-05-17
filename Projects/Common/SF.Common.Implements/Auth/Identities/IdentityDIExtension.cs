using SF.Auth.Identities.Internals;
using SF.Core.ServiceManagement;

namespace SF.Auth.Identities
{
	public static class IdentityDIExtension
	{
		public static IServiceCollection UseIdentity(
			this IServiceCollection sc
			)
		{
			sc.AddScoped<IIdentityService, IdentityService>();
			return sc;
		}
	}
}