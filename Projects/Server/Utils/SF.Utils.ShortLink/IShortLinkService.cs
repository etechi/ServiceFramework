using SF.Sys.Auth;
using SF.Sys.NetworkService;
using System;
using System.Threading.Tasks;

namespace SF.Utils.ShortLinks
{
	public class ShortLinkCreateArgument
	{
		/// <summary>
		/// 短链接名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 短链接目标
		/// </summary>
		public string Target { get; set; }

		/// <summary>
		/// POST参数
		/// </summary>
		public string PostData { get; set; }

		/// <summary>
		/// 过期时间
		/// </summary>
		public DateTime Expires { get; set; }
	}
	[NetworkService]
	[AnonymousAllowed]
	public interface IShortLinkService 
    {
		Task<System.Net.Http.HttpResponseMessage> Go(string Id);
    }
	public interface IShortLinkCreateService
	{
		Task<string> Create(ShortLinkCreateArgument Arg);
	}
}
