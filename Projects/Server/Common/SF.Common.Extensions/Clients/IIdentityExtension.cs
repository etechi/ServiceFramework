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

namespace SF.Clients
{
	public static class IClaimArrayExtension
	{
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
	}

}

