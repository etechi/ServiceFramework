using System.Threading.Tasks;

namespace SF.Externals.WeiXin.Mp
{
	public interface IAccessTokenManager
    {
        Task<string> GetAccessToken();
        Task<string> GetJsApiTicket();
    }
}
