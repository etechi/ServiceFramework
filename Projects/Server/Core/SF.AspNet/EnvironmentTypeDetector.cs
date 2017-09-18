﻿using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF
{

	public static class EnvironmentTypeDetector
	{
		public static EnvironmentType Detect()
		{
			
			return (EnvironmentType)Enum.Parse(typeof(EnvironmentType), System.Configuration.ConfigurationManager.AppSettings["EnvType"]);
		}
	}
}