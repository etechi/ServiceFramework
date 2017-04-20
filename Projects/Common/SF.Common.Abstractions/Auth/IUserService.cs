using SF.Auth.Identity.Models;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;
using SF.Auth.Identity;

namespace SF.Auth
{
	public interface IUserService
	{
		[Authorize]
		Task Update(UserDesc Desc);

		[Authorize]
		Task<UserDesc> GetCurUser();
	}

}

