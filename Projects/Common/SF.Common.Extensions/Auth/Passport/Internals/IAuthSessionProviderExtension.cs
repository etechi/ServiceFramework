using SF.Auth.Passport.Models;
using System;
using System.Threading.Tasks;

namespace SF.Auth.Passport.Internals
{
    public static class IAuthSessionProviderExtension
    {
		public static async Task<UserDesc> GetCurUser(this IAuthSessionProvider Provider)
		{
			return (await Provider.GetUserSession())?.User;
		}
		public static async Task<long?> GetCurUserId(this IAuthSessionProvider Provider)
		{
			return (await Provider.GetCurUser())?.Id;
		}
    }
}
