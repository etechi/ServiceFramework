using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF
{

	public static class EnvironmentTypeDetector
	{
		public static EnvironmentType Detect(IServiceCollection sc)
		{
			var he=(IHostingEnvironment)sc.Single(s=>s.ServiceType==typeof(IHostingEnvironment)).ImplementationInstance;
			if (he.IsDevelopment())
				return EnvironmentType.Development;
			else if (he.IsProduction())
				return EnvironmentType.Production;
			else if (he.IsStaging())
				return EnvironmentType.Staging;
			else if (he.EnvironmentName == "utils")
				return EnvironmentType.Utils;
			throw new NotSupportedException("不支持执行环境类型:" + he.EnvironmentName);
			//			return (EnvironmentType)Enum.Parse(typeof(EnvironmentType), System.Configuration.ConfigurationManager.AppSettings["EnvType"]);
		}
	}
}
