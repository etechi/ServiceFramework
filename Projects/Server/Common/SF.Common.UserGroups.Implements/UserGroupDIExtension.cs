using SF.Sys.Services;
using SF.Sys.Entities;
using SF.Sys.Services.Management;
using SF.Common.UserGroups.Managers;
using SF.Common.UserGroups.Models;
using SF.Sys.Settings;

namespace SF.Sys.Services
{
	public static class UserGroupDIExtension
	{
		public static IServiceCollection AddUserGroupSyncScope(this IServiceCollection sc)
		{
			sc.AddSingleton<GroupSyncScope>();


			return sc;
		}

		
	}
}
