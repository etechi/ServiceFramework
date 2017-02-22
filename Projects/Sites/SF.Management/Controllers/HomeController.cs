﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ManagementConsole.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
			HttpContext.Authentication.SignInAsync(null, null);

			return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
		public class Arg
		{
			public int a1 { get; set; }
			public int a2 { get; set; }
		}
		[HttpPost]
		public string Add(int a,[FromBody]Arg aa)
		{
			return "aaa";
		}
    }
}