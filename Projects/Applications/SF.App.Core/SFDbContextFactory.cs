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

namespace SF.Applications
{
	public class SFDbContextFactory : IDesignTimeDbContextFactory<SFDbContext>
	{
		IAppInstance Instance { get; } = Core2App.Setup(Core.Hosting.EnvironmentType.Utils).Build();

		public SFDbContext CreateDbContext(string[] args)
		{
			return Instance.ServiceProvider.Resolve<SFDbContext>();
		}
	}


}
