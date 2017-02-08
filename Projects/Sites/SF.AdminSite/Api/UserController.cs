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
		public UserController(ICalc calc)
		{
			this.calc = calc;
		}
		[HttpGet]
		public int Add(int a,int b)
		{
			return calc.Add(a,b);
		}

		

	}
}
