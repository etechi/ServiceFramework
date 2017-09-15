using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SF.Data;
using SF.Data.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using System.Data.Common;
using Microsoft.EntityFrameworkCore.Design;
using SF.Core.ServiceManagement;
using SF.Core.Hosting;

namespace Hygou
{
	public class HygouDbContextFactory : IDesignTimeDbContextFactory<HygouDbContext>
	{
		IAppInstance Instance { get; } = HygouApp.Setup(SF.Core.Hosting.EnvironmentType.Utils).Build();

		public HygouDbContext CreateDbContext(string[] args)
		{
			return Instance.ServiceProvider.Resolve<HygouDbContext>();
		}
	}


}
