using SF.Metadata;
using SF.Auth;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Entities;
using SF.Data;

namespace SF.Users.Members
{
	public static class MemberExtension
	{
		public static  Task MemberEnsure(
		   this IMemberManagementService ManagementService,
		   string name,
		   string nick,
		   string phoneNumber,
		   string password,
		   string[] roles = null,
		   bool phoneConfirmed = false,
		   int? inviter = null,
		   int? userSource = null
		   )
			{
			//var um = scope.Resolve<Bizness.Auth.UserManager>();
			//var user = await um.FindByNameAsync(name);
			//if (user != null)
			//{
			//	user.NickName = nick;
			//	user.PhoneNumber = phoneNumber;
			//	user.PhoneNumberConfirmed = phoneConfirmed;
			//	await um.UpdateAsync(user);
			//	return user;
			//}
			//user = new DataModels.User
			//{
			//	UserName = name,
			//	NickName = nick,
			//	PhoneNumber = phoneNumber,
			//	PhoneNumberConfirmed = phoneConfirmed,
			//	InviterUserId = inviter ?? 0,
			//	SourceId = userSource,
			//	Roles = (roles ?? Array.Empty<string>()).Select(r => new DataModels.UserRole { RoleId = r }).ToArray()
			//};
			//var re = await um.CreateAsync(user, password);
			//if (!re.Succeeded)
			//	throw new Exception(re.Errors.Join(";"));
			//return user;
			return null;
			}
	}

}

