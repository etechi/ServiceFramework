using SF.Auth;
using SF.Metadata;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media
{
    [Comment(Name = "媒体附件支持", GroupName = "媒体附件")]
	[NetworkService]
    public interface IMediaService
    {
		[Authorize]
		[HeavyMethod]
		Task<HttpResponseMessage> Upload(bool returnJson = false);

		[Authorize]
		Task<string> Clip(string src, double x, double y, double w, double h);

		Task<HttpResponseMessage> Get(string id, string format = null);


	}
}
