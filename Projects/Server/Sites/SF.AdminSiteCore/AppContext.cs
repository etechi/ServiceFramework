using Microsoft.EntityFrameworkCore;
using SF.Core.Hosting;
using System;

namespace SF.AdminSiteCore
{
	public class AppContext : DbContext
	{
		public AppContext(DbContextOptions<AppContext> options)
			: base(options)
		{ }

	}

}