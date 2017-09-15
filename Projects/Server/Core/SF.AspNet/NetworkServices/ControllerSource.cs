using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using SF.Core.Serialization;
using SF.Auth;
using SF.Metadata;
using System.IO;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http;

namespace SF.AspNet.NetworkService
{
	class ControllerSource
	{
		public ServiceController Controller
		{
			get;set;
		}
	}

}
