#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Auth.IdentityServices.Internals;
using SF.Auth.IdentityServices.Models;
using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
namespace SF.Auth.IdentityServices
{
	public class UserService :
		IUserService,
		IManagedServiceWithId
	{
		UserServiceSetting Setting { get; }

		public long? ServiceInstanceId => Setting.ServiceInstanceDescriptor.Value.InstanceId;

		public UserService(UserServiceSetting Setting)
		{
			this.Setting = Setting;
		}
		Task<UserData> GetIdentityData(long Id) =>
			Setting.IdentStorage.Value.Load(Id);
			//Setting.IdentityDataCache.Value.GetOrCreateAsync(
			//	  Id.ToString(),
			//	  () => Setting.IdentStorage.Value.Load(Id),
			//	  TimeSpan.FromHours(1)
			//	  );

		public async Task<User> GetUser(long Id)
		{
			var data = await GetIdentityData(Id);
			return data == null ? null : new User
			{
				Id = data.Id,
				Icon = data.Icon,
				Name = data.Name,
				//OwnerId = data.Entity
			};
		}
		public Task<long?> GetCurUserId()
		{
			//var re = Setting.AccessToken.Value;
			//return Task.FromResult(re?.User?.GetUserIdent());
			return Task.FromResult((long?)null);
		}
		public async Task<User> GetCurUser()
		{
			var id = await GetCurUserId();
			return id.HasValue ? await GetUser(id.Value) : null;
		}
		public async Task<long> EnsureCurUserId()
		{
			var id = await GetCurUserId();
			if (id.HasValue) return id.Value;
			throw new PublicDeniedException("未登录");
		}
		async Task<byte[]> LoadAccessTokenPassword(byte[] data,int offset)
		{
			var uid = BitConverter.ToInt64(data, offset);
			var idata = await GetIdentityData(uid);
			return idata.SecurityStamp;
		}
		public Task<long> ValidateAccessToken(string AccessToken)
		{
			//var p = Setting.AccessTokenHandler.Value.Validate(AccessToken);
			//var id = p.GetUserIdent();
			//if (!id.HasValue)
			//	throw new PublicArgumentException("访问令牌问包含用户ID");
			//return Task.FromResult(id.Value);
			return Task.FromResult(0L);
		}


		async Task<string> SendVerifyCode(IUserCredentialProvider CredentialProvider,ConfirmMessageType Type, string Ident, long? UserId, string BizIdent)
		{
			var code = Strings.Numbers.Random(6);

			Setting.VerifyCodeCache.Value.Set(
				$"{ConfirmMessageType.PasswordRecorvery}\n{Ident}\n{UserId}",
				new VerifyCode
				{
					Type = ConfirmMessageType.PasswordRecorvery,
					Code = code,
					UserId = UserId
				},
				Setting.TimeService.Value.Now.AddMinutes(15)
				);
			if (Setting.VerifyCodeVisible)
				return code;

			await CredentialProvider.SendConfirmCode(
				UserId,
				Ident,
				code,
				Type,
				BizIdent
				);

			return null;
		}
		VerifyCode CheckVerifyCode(ConfirmMessageType Type,string Ident,long UserId,string Code)
		{
			var verifyCode = Setting.VerifyCodeCache.Value.Get(
				$"{Type}\n{Ident}\n{UserId}"
				);
			if (verifyCode == null)
				throw new PublicInvalidOperationException("请先发送验证码");
			if (verifyCode.Code != Code)
				throw new PublicDeniedException("验证码错误");
			return verifyCode;
		}


		public async Task<string> ResetPasswordByRecoveryCode(ResetPasswordByRecorveryCodeArgument Arg)
		{
			var vc = CheckVerifyCode(ConfirmMessageType.PasswordRecorvery, Arg.Credential, 0, Arg.VerifyCode);

			var stamp = Bytes.Random(16);
			var newPasswordHash = Setting.PasswordHasher.Value.Hash(Arg.NewPassword, stamp);
			await Setting.IdentStorage.Value.UpdateSecurity(vc.UserId.Value, newPasswordHash, stamp);
			return await SetOrReturnAccessToken(
				vc.UserId.Value,
				null,
				Arg.ReturnToken
				);
		}

		public async Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg)
		{
			var provider = GetCredentialProvider(Arg.CredentialProvider);

			var err = await provider.VerifyFormat(Arg.Credential);
			if (err != null)
				throw new PublicArgumentException(err);

			var bind = await Setting.CredentialStorage.Value.Find(provider.Ident,  Arg.Credential);
			if (bind == null)
				throw new PublicArgumentException("找不到账号");
			return await SendVerifyCode(
				provider,
				ConfirmMessageType.PasswordRecorvery, 
				Arg.Credential, 
				bind.UserId, 
				null
				);
		}

		public async Task<string> SetPassword(SetPasswordArgument Arg)
		{
			var uid = await EnsureCurUserId();
			var iddata = await GetIdentityData(uid);
			var hash = iddata.PasswordHash;
			var oldPasswordHash = Setting.PasswordHasher.Value.Hash(Arg.OldPassword,iddata.SecurityStamp);
			if (oldPasswordHash != hash)
				throw new ArgumentException("旧密码错误");

			var stamp = Bytes.Random(16);
			var newPasswordHash = Setting.PasswordHasher.Value.Hash(Arg.NewPassword,stamp);
			await Setting.IdentStorage.Value.UpdateSecurity(uid, newPasswordHash, stamp);
			return await SetOrReturnAccessToken(
				uid,
				null,
				Arg.ReturnToken
				);
		}
		protected DateTime? GetExpires(int? Expires) =>
			Expires.HasValue ? (DateTime?)Setting.TimeService.Value.Now.AddSeconds(Expires.Value) : null;

		public async Task<string> Signin(SigninArgument Arg)
		{
			if (string.IsNullOrWhiteSpace(Arg.Ident))
				throw new PublicArgumentException("请输入用户标识");

			if (string.IsNullOrWhiteSpace(Arg.Password))
				throw new PublicArgumentException("请输入密码");


			UserCredential ui = null;
			IUserCredentialProvider identProvider = null;
			var credentialStorage = Setting.CredentialStorage.Value;
			foreach (var ip in Setting.IdentityCredentialProviders)
			{
				if (await ip.VerifyFormat(Arg.Ident) != null)
					continue;
				ui = await credentialStorage.Find(ip.Ident, Arg.Ident);
				if (ui != null)
				{
					identProvider = ip;
					break;
				}
			}
			if (ui == null)
				throw new PublicArgumentException("用户或密码错误！");
			var idData = await GetIdentityData(ui.UserId);
			var passwordHash = idData.PasswordHash;
			if (Setting.PasswordHasher.Value.Hash(Arg.Password, idData.SecurityStamp) != passwordHash)
			{
				//await Setting.UserStorage.SigninFailed(
				//	ui.UserId,
				//	3,
				//	TimeSpan.FromMinutes(10),
				//	Setting.AccessInfo.Value.Value
				//	);

				throw new PublicArgumentException("用户或密码错误!");
			}

			if (!idData.IsEnabled)
				throw new PublicDeniedException("用户已被禁止登录");

			//await Setting.UserStorage.SigninSuccess(
			//	ui.UserId,
			//	Setting.AccessInfo.Value.Value
			//	);
			return await SetOrReturnAccessToken(ui.UserId, Arg.Expires, Arg.ReturnToken);
		}

		async Task<string> SetOrReturnAccessToken(long UserId,int? Expires,bool ReturnToken)
		{
			var DateExpires = GetExpires(Expires);
			if (ReturnToken)
				return await CreateAccessToken(UserId, DateExpires);
			await Setting.ClientService.Value.SignInAsync(
				await CreatePrincipal(UserId),
				Expires.HasValue?(DateTime?)Setting.TimeService.Value.Now.AddSeconds(Expires.Value):null
				);
			return null;
		}
		async Task<ClaimsPrincipal> CreatePrincipal(long Id)
		{
			var data = await Setting.IdentStorage.Value.Load(Id);

			return new ClaimsPrincipal(
				new ClaimsIdentity(
					data.Claims.Select(c => new Claim(c.TypeName, c.Value)).WithFirst(
						new Claim("id",data.Id.ToString()),
						new Claim(ClaimTypes.Name, data.Name),
						new Claim(ClaimTypes.Role, data.Roles.Join(" "))
						),
					"SFAuth"
					)
				);

		}
		public Task<string> CreateAccessToken(long Id,DateTime? Expires)
		{
			//return Setting.AccessTokenHandler.Value.Create(
			//	await CreatePrincipal(Id),
			//	Expires
			//	);
			return Task.FromResult((string)null);
		}
		public Task Signout()
		{
			return Setting.ClientService.Value.SignOutAsync();
		}

		IUserCredentialProvider GetCredentialProvider(string Provider) =>
			Setting.CredentialProviderResolver(Provider ?? Setting.DefaultIdentityCredentialProvider);


		public async Task<string> SendCreateIdentityVerifyCode(SendCreateIdentityVerifyCodeArgument Arg)
		{
			var CredentialProvider = GetCredentialProvider(Arg.CredentialProvider);

			var err=await CredentialProvider.VerifyFormat(Arg.Credetial);
			if (err != null)
				throw new PublicArgumentException(err);

			return await SendVerifyCode(
				CredentialProvider,
				ConfirmMessageType.Signup,
				Arg.Credetial,
				null,
				null
				);
		}

		public async Task<string> Signup(SignupArgument Arg, bool VerifyCode)
		{
			if (string.IsNullOrWhiteSpace(Arg.Credential))
				throw new PublicArgumentException("请输入用户标识");

			var CredentialProvider = GetCredentialProvider(Arg.CredentialProvider);

			var msg = await CredentialProvider.VerifyFormat(Arg.Credential);
			if (msg != null)
				throw new ArgumentException(msg);
			
			var canSendMessage = CredentialProvider.IsConfirmable();
			if (canSendMessage)
				CheckVerifyCode(
					ConfirmMessageType.Signup, 
					Arg.Credential, 
					0, 
					Arg.VerifyCode
					);


			var ui = await Setting.CredentialStorage.Value.Find(CredentialProvider.Ident,Arg.Credential);
			if (ui != null)
				throw new PublicArgumentException($"您输入的{CredentialProvider.Name}已被注册");


			//ui = await CredentialProvider.FindOrBind(
			//	Arg.Credential,
			//	null,
			//	canSendMessage,
			//	uid
			//	);
			//if (ui.IdentityId != uid)
			//	throw new PublicArgumentException($"您输入的{CredentialProvider.Name}已被注册");

			if (string.IsNullOrWhiteSpace(Arg.Password))
				throw new PublicArgumentException("请输入密码");
			var stamp = Bytes.Random(16);
			var passwordHash = Setting.PasswordHasher.Value.Hash(Arg.Password,stamp);
			var client = Setting.ClientService.Value;
			var uid = await Setting.IdentStorage.Value.Create(
				new UserCreateArgument
				{
					AccessSource = client.UserAgent,
					PasswordHash = passwordHash,
					SecurityStamp = stamp,
					CredentialValue=Arg.Credential,
					ClaimTypeId= CredentialProvider.Ident,
					ExtraArgument=Arg.ExtraArgument,
					User =new User
					{
					//	OwnerId = Arg.User.OwnerId,
						Icon = Arg.User.Icon,
						Name = Arg.User.Name
					}
				});
			return await SetOrReturnAccessToken(uid, Arg.Expires, Arg.ReturnToken);
		}

		public async Task Update(User Identity)
		{
			var cid = await GetCurUserId();
			if (!cid.HasValue)
				throw new PublicDeniedException("用户未登录");
			if (Identity.Id == 0)
				Identity.Id = cid.Value;
			else if (Identity.Id != cid.Value)
				throw new PublicDeniedException("禁止访问");

			await Setting.IdentStorage.Value.UpdateDescription(Identity);
		}
	}

}
