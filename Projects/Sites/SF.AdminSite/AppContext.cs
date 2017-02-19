using SF.Core.Hosting;
using System;

namespace SF.AdminSite
{
	public class AppContext : SF.Data.Storage.DbContext
	{
		public AppContext() : this(
			new AppInstanceBuilder(EnvironmentType.Utils)
			.Build()
			.ServiceProvider
			)
		{

		}
		public AppContext(IServiceProvider sp) : base(sp, "name = default")
		{

		}
	}
}