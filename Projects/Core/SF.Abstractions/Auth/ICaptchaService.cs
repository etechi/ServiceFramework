
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SF.Auth
{
    public interface IAuthCaptchaService
    {
		Task<HttpResponseMessage> Image(string Type, int Width = 100, int Height = 30, string FColor = "#000000", string BColor = "#ffffff");
		Task<bool> Verify(string Type, string Code);
	}
}
