using Microsoft.AspNetCore.Hosting;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SF.AspNetCore
{
	public class DefaultFilePathStructure : IDefaultFilePathStructure
	{
		IHostingEnvironment Environment { get; }
		public string BinaryPath => System.IO.Path.Combine(Environment.ContentRootPath, "bin");

		public FilePathDefination FilePathDefination=>new FilePathDefination();

		public string RootPath => System.IO.Path.Combine(Environment.ContentRootPath, "root");
		public DefaultFilePathStructure(IHostingEnvironment env)
		{
			Environment = env;
		}
	}
}
