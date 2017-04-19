using SF.System.Auth.Identity.Models;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;
using System;
using SF.Clients;
using SF.System.Auth.Identity;

namespace SF.System.Auth
{
	public abstract class UserService<TSetting,TDesc> : IUserService
		where TSetting : UserServiceSetting
		where TDesc: Identity.Models.IdentDesc,new()
	{
		class Session : ISession,IIdentity
		{
			public string Id { get; set; }
			public IIdentity User => this;
			public Claim[] Claims { get; set; } 
		}

		public TSetting Setting { get; }
		public UserService(TSetting Setting)
		{
			this.Setting = Setting;
		}
		public long? GetUserIdent(int ScopeId)
		{
			return Setting.ClientService.GetSession(ScopeId)?.User?.Claims?.GetUserIdent();
		}
		public long? GetSessionIdent(int ScopeId)
		{
			var re= Setting.ClientService.GetSession(ScopeId)?.Id;
			if (re == null)
				return null;
			return long.Parse(re);
		}
		public abstract Task<TDesc> GetUserDesc(long Id);

		public long EnsureUserIdent(int ScopeId)
		{
			var uid = GetUserIdent(ScopeId);
			if (uid.HasValue)
				return uid.Value;
			throw new PublicDeniedException("未登录");
		}
		public Task<IdentDesc> GetCurIdent(int ScopeId)
		{
			var claims = Setting.ClientService.GetSession(ScopeId)?.User?.Claims;
			if (claims == null)
				return null;

			var desc = new TDesc();
			claims.FillIdentDesc(desc);
			return Task.FromResult((IdentDesc)desc);
		}

		protected async Task<TDesc> BindSession(
			int ScopeId,
			string IdentValue,
			string IdentProvider,
			DateTime? Expires,
			Func<Task<string>> AccessTokenLoader
			)
		{
			return await Setting.SessionService.Value.Create(
				ScopeId,
				IdentValue,
				IdentProvider,
				Setting.ClientService.AccessSource,
				async SessionCreator =>
				{
					var token = await AccessTokenLoader();
					var uid = await Setting.IdentService.Value.ParseAccessToken(token);
					var desc = await GetUserDesc(uid);
					var sid = await SessionCreator(uid);
					await this.Setting.ClientService.BindSession(
						ScopeId,
						new Session
						{
							Id=sid,
							Claims = desc.ToClaims()
						},
						Expires
						);
					return desc;
				}
				);
		}
		public async Task<string> ResetPasswordByRecoveryCode(ResetPasswordByRecorveryCodeArgument Arg)
		{
			var sess=await Setting.IdentService.Value.ResetPasswordByRecoveryCode(Arg);
			await BindSession(Arg.ScopeId, null, null, null, () => Task.FromResult(sess));
			return sess;
		}

		public Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg)
		{
			return Setting.IdentService.Value.SendPasswordRecorveryCode(Arg);
		}

		public async Task SetPassword(SetPasswordArgument Arg)
		{
			await Setting.IdentService.Value.SetPassword(
				new Identity.SetPasswordArgument
				{
					NewPassword=Arg.NewPassword,
					OldPassword=Arg.OldPassword,
					UserId= EnsureUserIdent(Arg.ScopeId)
				});
		}
		protected DateTime? GetExpires(int? Expires)=>
			Expires.HasValue? (DateTime?) Setting.TimeService.Value.Now.AddSeconds(Expires.Value) : null;

		public async Task<IdentDesc> Signin(SigninArgument SigninArgument)
		{
			var Expires = GetExpires(SigninArgument.Expires);

			return await BindSession(
				SigninArgument.ScopeId,
				SigninArgument.Ident,
				null,
				Expires,
				() =>
					Setting.IdentService.Value.CreateAccessToken(
						new CreateAccessTokenArgument
						{
							Ident = SigninArgument.Ident,
							Password = SigninArgument.Password,
							Expires = SigninArgument.Expires
						}
						)
				);
		}

		public async Task<IdentDesc> SigninByAccessToken(int ScopeId,string AccessToken, int? Expires)
		{
			var ExpiresDateTime = GetExpires(Expires);
			return await BindSession(
				ScopeId,
				null,
				null,
				ExpiresDateTime,
				() =>Task.FromResult(AccessToken)					
				);
		}

		public Task Signout(int ScopeId)
		{
			var sid = GetSessionIdent(ScopeId);
			if (sid != null)
			{
				Setting.ClientService.ClearSession(ScopeId);
				Setting.SessionService.Value.Signout(sid.Value);
			}
			return Task.CompletedTask;
		}

		public abstract Task Update(IdentDesc Desc);

	}

}

