﻿using System;
using System.Collections.Generic;
using System.Linq;
using SF.Core.ServiceManagement;
using SF.Metadata;
using SF.Services.Test;
using SF.Core.TaskServices;
using SF.Core.Hosting;
using SF.Core.Logging;
using Microsoft.Extensions.Logging;
using SF.Management.MenuServices.Models;
using SF.Core.ServiceManagement.Management;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using SF.Data.EntityFrameworkCore;
using Hygou.Setup;

namespace Hygou
{	
	public static class HygouDIExtensions
	{
		public static IServiceCollection AddHygouServices(this IServiceCollection sc,EnvironmentType envType)
		{
			sc.AddInitializer("data", "初始化Hygou数据", sp => SystemInitializer.Initialize(sp, envType));
			return sc;
		}
	
	}
}