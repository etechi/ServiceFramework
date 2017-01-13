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
	public class UserProvider<TUser> : 
		IUserProvider
		where TUser:DataModels.User
	{
		public IDataContext Context { get; }
		public IIdentGenerator IdentGenerator { get; }
		public UserProvider(IAuthSessionProvider AuthSessionProvider, IDataContext Context)
		{
			this.Context = Context;
		}

		
		public virtual async Task Update(UserInfo User)
		{
			var u = await Context.Editable<TUser>().FindAsync(User.Id);
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

		public virtual Task<UserInfo> FindById(long UserId)
		{
			return Context
				.ReadOnly<TUser>()
				.Where(u => u.Id == UserId && u.ObjectState == LogicObjectState.Enabled)
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

		public virtual Task<UserInfo> Create(UserCreateArgument Arg)
		{
			
		}

		public virtual Task<bool> VerifyPassword(long UserId, string Password)
		{
			throw new NotImplementedException();
		}

		public Task Signin(long UserId, ClientAccessInfo AccessInfo)
		{
			throw new NotImplementedException();
		}
	}
}
