using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.AspNetCore;
using SF.Services.Settings;

namespace Hygou.Site.Controllers 
{
	public class AdminController : Controller
	{
		
        public ActionResult Bizness()
		{
			return View();
		}
		public ActionResult System()
		{
			return View();
		}
	}
}
