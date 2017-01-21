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
		public IDataSet<TUser> DataSet { get; }
		public UserProvider(IAuthSessionProvider AuthSessionProvider, IDataSet<TUser> DataSet)
		{
			this.DataSet = DataSet;
		}

		
		public virtual async Task UpdateAsync(UserInfo User)
		{
			var u = await DataSet.FindAsync(User.Id);
			u.NickName = User.NickName.Trim();
			if (string.IsNullOrWhiteSpace(u.NickName)) throw new PublicArgumentException("请输入昵称");
			if(u.NickName.Length < 2) throw new PublicArgumentException("昵称太短");
			if (u.NickName.Length > 20) throw new PublicArgumentException("昵称太长");

			u.Icon = User.Icon;
			u.Image = User.Image;
			u.Sex = User.Sex;
			DataSet.Update(u);
			await DataSet.Context.SaveChangesAsync();
		}

		public virtual Task<UserInfo> FindById(long UserId)
		{
			return DataSet
				.AsQueryable(true)
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

		public Task<UserInfo> Create(UserCreateArgument Arg)
		{
			return null;
		}

		public Task<string> GetPasswordHash(long UserId, bool ForSignin)
		{
			throw new NotImplementedException();
		}

		public Task SetPasswordHash(long UserId, string PasswordHash, string SecurityStamp)
		{
			throw new NotImplementedException();
		}

		public Task<UserInfo> Signin(long UserId, bool Success, ClientAccessInfo AccessInfo)
		{
			throw new NotImplementedException();
		}
	}
}
