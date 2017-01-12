using ServiceProtocol.Data.Entity;
using SF.Data;
using SF.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Auth.Users.EFCore
{
	public class UserService<TUser> : 
		IUserService
		where TUser:DataModels.User
	{
		public IAuthSessionProvider AuthSessionProvider { get; }
		public IDataContext Context { get; }
		public UserService(IAuthSessionProvider AuthSessionProvider, IDataContext Context)
		{
			this.AuthSessionProvider = AuthSessionProvider;
			this.Context = Context;
		}

		protected Task<UserInfo> LoadUserInfo(long Id)
		{
			return Context
				.ReadOnly<TUser>()
				.Where(u => u.Id == Id && u.ObjectState == LogicObjectState.Enabled)
				.Select(u => new UserInfo
				{
					Id = u.Id,
					Icon = u.Icon,
					Image = u.Image,
					NickName = u.NickName,
					Sex = u.Sex,
					Type = u.UserType
				}).SingleOrDefaultAsync();
		}
		public async Task<UserInfo> GetCurUser()
		{
			var uid = await AuthSessionProvider.GetCurrentUserId();
			if (uid == null)
				throw new PublicInvalidOperationException("没有登录");
			return await LoadUserInfo(uid.Value);
		}

		public async Task Update(UserInfo User)
		{
			var uid = await AuthSessionProvider.GetCurrentUserId();
			if (uid == null)
				throw new PublicInvalidOperationException("没有登录");
			if (uid.Value != User.Id)
				throw new PublicDeniedException("不能修改其他人的用户信息");
			var u = await Context.Editable<TUser>().FindAsync(uid);
			u.NickName = User.NickName.Trim();
			if (string.IsNullOrWhiteSpace(u.NickName)) throw new PublicArgumentException("请输入昵称");
			if(u.NickName.Length < 2) throw new PublicArgumentException("昵称太短");
			if (u.NickName.Length > 20) throw new PublicArgumentException("昵称太长");

			u.Icon = User.Icon;
			u.Image = User.Image;
			u.Sex = User.Sex;
			Context.Update(u);
			await Context.SaveChangesAsync();
		}
	}
}
