using System;

namespace SF.AdminSite
{
	public class AppContext : SF.Data.Storage.DbContext
	{
		public AppContext() : this(new ServiceConfiguration(EnvironmentType.Utils).ServiceProvider)
		{

		}
		public AppContext(IServiceProvider sp) : base(sp, "name = default")
		{

		}
	}
}