using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SF.AspNet
{
	public class DefaultFilePathStructure : IDefaultFilePathStructure
	{
		public string BinaryPath { get; } 

		public FilePathDefination FilePathDefination=>new FilePathDefination();

		public string RootPath { get; }
		public DefaultFilePathStructure()
		{
			RootPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data");
			BinaryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/bin");
		}
	}
}
