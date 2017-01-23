using SF.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public interface IAuthSessionProvider
    {
		Task<long?> GetCurrentUserId();
		Task BindUser(UserInfo user);
		Task UnbindUser();
	}
}

