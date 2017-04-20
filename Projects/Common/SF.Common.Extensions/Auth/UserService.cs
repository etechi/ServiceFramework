using SF.Auth.Identity.Models;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;
using System;
using SF.Clients;
using SF.Auth.Identity;

namespace SF.Auth
{
	public abstract class UserService<TSetting,TDesc> : IUserService
		where TSetting : UserServiceSetting
		where TDesc: UserDesc,new()
	{
		public TSetting Setting { get; }
		public UserService(TSetting Setting)
		{
			this.Setting = Setting;
		}
		public Task<long?> GetUserId()
		{
			return Setting.IdentService.GetCurUserId();
		}
		public abstract Task<TDesc> GetUserDesc(long Id);

		public async Task<long> EnsureUserId()
		{
			var uid = await GetUserId();
			if (uid.HasValue)
				return uid.Value;
			throw new PublicDeniedException("未登录");
		}
		
		public abstract Task Update(UserDesc Desc);

		public async Task<UserDesc> GetCurUser()
			=> await GetUserDesc(await EnsureUserId());
	}

}

