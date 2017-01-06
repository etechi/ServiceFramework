using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Security.Principal;
using System.Security.Claims;

namespace SF.Auth
{
	public static class IPrincipalExtension {
		public static readonly string PermissionClaimType = "SP.PMS";

		static public bool CheckPermission(this IPrincipal principal,string Resource,string Operation)
		{
			var ci = principal.Identity as ClaimsIdentity;
			if (ci == null)
				return false;
			var permission = (Resource + ":" + Operation).ToLower();
			return ci.Claims.Any(c => c.Type == PermissionClaimType && c.Value == permission);
		}
	}
}
