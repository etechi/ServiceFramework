using SF.Core.Hosting;
using System;

namespace SF.Applications
{
	public class AppContext : SF.Data.Storage.DbContext
	{
		public AppContext() : this(
			 new AppInstanceBuilder(EnvironmentType.Utils).Build().ServiceProvider
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