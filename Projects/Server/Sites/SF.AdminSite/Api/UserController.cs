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
		public HttpResponseMessage Test()
		{
			return HttpResponse.Text("asasdas");
		}
		

	}
}
