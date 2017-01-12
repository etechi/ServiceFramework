using ServiceProtocol;
using SF.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	[NetworkService]
	[Authorize]
	public interface IUserService
    {
        Task<UserInfo> GetCurUser();
        Task Update(UserInfo User);
    }

}

