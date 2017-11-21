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

using SF.Sys;
using SF.Sys.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Auth
{
	public static class ClaimsPrincipalExtensions
	{
		public static long? GetUserIdent(this ClaimsPrincipal user)
		{
			if (user?.Identity?.IsAuthenticated ?? false)
				return user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value?.TryToInt64();
			return null;
		}
		public static Claim CreateGrantClaim(IEnumerable<(string res,string action)> grants)
		{
			var str = grants.Select(g => g.res + ":" + g.action).Join("|");
			if (str.Length == 0)
				return null;
			return new Claim("grant","|" + str + "|" );
		}
		public static void OperatorValidate(this ClaimsPrincipal user, long OperatorId)
		{
			if (!(user?.Identity?.IsAuthenticated ?? false))
				throw new PublicNotSigninException();
			if (user?.IsInRole("root") ?? false)
				return;
			if(user.GetUserIdent()!=OperatorId)
				throw new PublicDeniedException("没有权限");
		}
		public static void PermissionValidate(this ClaimsPrincipal user,string resource,string action)
		{
			if (!(user?.Identity?.IsAuthenticated ?? false))
				throw new PublicNotSigninException();
			if (user?.IsInRole("root") ?? false)
				return;
			var grant = "|" + resource + ":" + action + "|";
			if(!user.HasClaim(c=>c.Type == "grant" && c.Value.Contains(grant)))
				throw new PublicDeniedException("没有权限");
		}
	}
}
