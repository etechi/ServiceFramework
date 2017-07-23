using SF.Core.Hosting;
using System;

namespace SF.AdminSite
{
	public class AppContext : SF.Data.Storage.DbContext
	{
		public AppContext() : this(
			 AppInstanceBuilder.Build(EnvironmentType.Utils)
			.ServiceProvider
			)
		{

		}
		public AppContext(IServiceProvider sp) : base(sp, "name = default")
		{

		}
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}