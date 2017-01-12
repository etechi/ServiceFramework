
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Auth
{

    public interface IAuthIdentVerifyService
    {
		Task SendCode(string IdentVerifyServiceId, string Ident,string Purpose);
		Task<bool> Verify(string IdentVerifyServiceId, string Ident, string Purpose, string Code);
	}
}
