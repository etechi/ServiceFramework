using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;
using System;
using System.Linq;
using System.Collections.Generic;
using SF.Auth.Identities.Models;
using SF.Entities;
using SF.Core;
using SF.Core.ServiceManagement;

namespace SF.Auth.Identities
{
	public static class IIdentityServiceExtension
	{
		public static async Task<long> EnsureCurIdentityId(this IIdentityService IdentityService)
		{
			var re = await IdentityService.GetCurIdentityId();
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
		public static async Task<long> IdentityEnsure(
			this IIdentityService IdentityService,
			IServiceProvider ServiceProvider,
			string name,
			string nick,
			string phoneNumber,
			string password,
			string entity,
			string[] roles = null,
			Dictionary<string,string> extArgs=null
			)
		{
			long iid=0;
			var sid = ((IManagedServiceWithId)IdentityService).ServiceInstanceId;
			var ims = ServiceProvider.Resolve<IIdentityManagementService>(null, sid);
			if (phoneNumber != null)
				iid = (await ims.QueryIdentsAsync(new IdentityQueryArgument
				{
					Ident = phoneNumber,
				}, Paging.Single)).Items.FirstOrDefault();

			if(iid==0 && name!=null)
				iid = (await ims.QueryIdentsAsync(new IdentityQueryArgument
				{
					Ident = name
				}, Paging.Single)).Items.FirstOrDefault();

			if (iid > 0)
			{
				await ims.UpdateEntity(iid, e => {
					e.Name = nick;
				});
				return iid;
			}
			var ics = new List<IdentityCredential>();
			if (name != null)
				ics.Add(new IdentityCredential
				{
					Credential = name,
					ProviderId = ServiceProvider.Resolver().ResolveDescriptorByType(sid, typeof(IIdentityCredentialProvider), "账号认证").InstanceId
				});
			if (phoneNumber != null)
				ics.Add(new IdentityCredential
				{
					Credential = phoneNumber,
					ProviderId = ServiceProvider.Resolver().ResolveDescriptorByType(sid, typeof(IIdentityCredentialProvider), "手机号认证").InstanceId
				});

			if (roles != null && roles.Length > 0)
			{
				if (extArgs == null)
					extArgs = new Dictionary<string, string>();
				extArgs["roles"] = Json.Stringify(roles);
			}

			var sess = await IdentityService.CreateIdentity(
				new CreateIdentityArgument
				{
					Credential = ics[0].Credential,
					CredentialProviderId = ics[0].ProviderId,
					ExtraArgument = extArgs.Count > 0 ? extArgs : null,
					Identity = new Identity
					{
						OwnerId = entity,
						Icon = null,
						Name = nick
					},
					Password = password,
					ReturnToken = true
				},
				false
				);
			return await IdentityService.ParseAccessToken(sess);

		}
		
	}

}

