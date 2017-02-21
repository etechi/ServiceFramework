using SF.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace SF.AdminSite.Api
{
    public class UserController : ApiController
    {
		ICalc calc { get; }
		ILogger Logger { get; }
		public UserController(ICalc calc, ILogger<UserController> logger)
		{
			this.calc = calc;
			this.Logger = logger;
		}
		[HttpGet]
		public int Add(int a,int b)
		{
			//this.ActionContext.RequestContext.
			Logger.Info("{a,0}+{b,1}={re,2}", a, b, a + b);
			return calc.Add(a,b);
		}
		public HttpResponseMessage Test()
		{
			return HttpResponse.Text("asasdas");
		}
		

	}
}
