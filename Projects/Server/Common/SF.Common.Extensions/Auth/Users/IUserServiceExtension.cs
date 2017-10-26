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

using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;
using System;
using System.Linq;
using System.Collections.Generic;
using SF.Auth.Users.Models;
using SF.Entities;
using SF.Core;
using SF.Core.ServiceManagement;
using SF.Auth.Users.Internals;

namespace SF.Auth.Users
{
	public static class IUserServiceExtension
	{
		public static async Task<long> EnsureCurIdentityId(this IUserService IdentityService)
		{
			var re = await IdentityService.GetCurUserId();
			if (re.HasValue)
				return re.Value;
			throw new PublicNotSigninException();
		}
		//public const string ClaimKeyScopeId = "scope";
		//public const string ClaimKeyUserId  = "id";
		//public const string ClaimKeyUserNickName = "name";
		//public const string ClaimKeyUserIcon  = "icon";
		//public const string ClaimKeyUserImage = "image";
		//public static Claim[] ToClaims(this IdentDesc desc)
		//{
		//	var re = new List<Claim>()
		//	{
		//		new Claim{Type=ClaimKeyUserId,Value=desc.Id.ToString()},
		//		new Claim{Type=ClaimKeyUserNickName,Value=desc.NickName},
		//		new Claim{Type=ClaimKeyScopeId,Value=desc.ScopeId.ToString()}
		//	};
		//	if (desc.Icon != null)
		//		re.Add(new Claim { Type = ClaimKeyUserIcon, Value = desc.Icon });
		//	if (desc.Image != null)
		//		re.Add(new Claim { Type = ClaimKeyUserImage, Value = desc.Image });
		//	return re.ToArray();
		//}
		//public static long? GetUserIdent(this Claim[] Claims)
		//{
		//	var v = Claims?.SingleOrDefault(c => c.Type == ClaimKeyUserId)?.Value;
		//	if (v == null)
		//		return null;
		//	return long.Parse(v);
		//}
		//public static void FillIdentDesc(this Claim[] Claims, IdentDesc Desc)
		//{
		//	if (Claims == null)
		//		return;
		//	foreach(var c in Claims)
		//	{
		//		switch (c.Type)
		//		{
		//			case ClaimKeyUserId:
		//				Desc.Id = long.Parse(c.Value);
		//				break;
		//			case ClaimKeyScopeId:
		//				Desc.ScopeId = int.Parse(c.Value);
		//				break;
		//			case ClaimKeyUserNickName:
		//				Desc.NickName = c.Value;
		//				break;
		//			case ClaimKeyUserIcon:
		//				Desc.Icon = c.Value;
		//				break;
		//			case ClaimKeyUserImage:
		//				Desc.Image = c.Value;
		//				break;

		//		}
		//	}
		//}

		//public static async Task<Bizness.Auth.Models.AdminEditable> AdminEnsure(
		//   this IDIScope scope,
		//   string name,
		//   string[] roles,
		//   string password
		//   )
		//{
		//	var adm = new Bizness.Auth.Models.AdminEditable
		//	{
		//		NickName = name,
		//		Password = password,
		//		Roles = roles,
		//		UserName = name
		//	};
		//	var m = scope.Resolve<Bizness.Auth.IAdminManager>();
		//	return await m.Ensure().WithModel<DataModels.User>(
		//		t => t.UserName == adm.UserName,
		//		o =>
		//		{
		//			adm.Id = o.Id;
		//			PropertryCopier.CopyTo(adm, o);
		//		}
		//		);
		//}
		public static async Task<long> UserEnsure<TUserManager,TInternal, TEditable, TQueryArgument>(
			this TUserManager UserManager,
			IServiceProvider ServiceProvider,
			string name,
			string nick,
			string phoneNumber,
			string password,
			string entity,
			string[] roles = null,
			Dictionary<string,string> extArgs=null
			)
			where TUserManager:IUserManager<TInternal,TEditable,TQueryArgument>
			where TInternal:UserInternal
			where TEditable : UserEditable
			where TQueryArgument : UserQueryArgument,new()
		{
			
			ObjectKey<long> iid=null;

			if (phoneNumber != null)
				iid = (await UserManager.QueryIdentsAsync(new TQueryArgument
				{
					Ident = phoneNumber,
				}, Paging.Single)).Items.FirstOrDefault();

			if(iid==null && name!=null)
				iid = (await UserManager.QueryIdentsAsync(new TQueryArgument
				{
					Ident = name
				}, Paging.Single)).Items.FirstOrDefault();

			if (iid !=null)
			{
				await UserManager.UpdateEntity(iid, e => {
					e.Name = nick;
				});
				return iid.Id;
			}
			var sid = ((IManagedServiceWithId)UserManager).ServiceInstanceId;
			var ics = new List<UserCredential>();
			if (name != null)
				ics.Add(new UserCredential
				{
					Credential = name,
					ProviderId = ServiceProvider.Resolver().ResolveDescriptorByType(sid, typeof(IUserCredentialProvider), "账号认证").InstanceId
				});
			if (phoneNumber != null)
				ics.Add(new UserCredential
				{
					Credential = phoneNumber,
					ProviderId = ServiceProvider.Resolver().ResolveDescriptorByType(sid, typeof(IUserCredentialProvider), "手机号认证").InstanceId
				});

			var UserService = (IUserService)ServiceProvider.Resolver().ResolveDescriptorByType(sid, typeof(IUserService), null);

			var sess = await UserService.Signup(
				new SignupArgument
				{
					Credential = ics[0].Credential,
					CredentialProviderId = ics[0].ProviderId,
					ExtraArgument = extArgs.Count > 0 ? extArgs : null,
					User = new User
					{
						Icon = null,
						Name = nick
					},
					Password = password
				},
				false
				);
			return await UserService.ValidateAccessToken(sess);

		}
		
	}

}

