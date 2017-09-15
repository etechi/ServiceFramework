using SF.Core;
using SF.Core.Caching;
using SF.Metadata;
using SF.Core.Drawing;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Test
{
	[Comment(Name = "测试服务", GroupName = "系统服务")]
    public class TestService : ITestService
    {
		public Task<HttpResponseMessage> Test1()
		{
			return Task.FromResult(HttpResponse.Text("test1111"));
		}
		public HttpResponseMessage Test2()
		{
			return HttpResponse.Text("test2222");
		}
	}
}
