using SF.Auth;
using SF.Metadata;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Test
{
    [Comment(Name = "测试服务", GroupName = "媒体附件")]
	[NetworkService]
    public interface ITestService
    {
		

		Task<HttpResponseMessage> Test1();
		HttpResponseMessage Test2();

	}
}
