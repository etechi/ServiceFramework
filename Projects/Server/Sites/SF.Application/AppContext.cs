using SF.Core.Hosting;
using System;
using SF.Core.ServiceManagement;

namespace SF.Applications
{
	public class AppContext : SF.Data.Storage.DbContext
	{
		public AppContext() : this(
			 App.Builder(EnvironmentType.Utils).Build().ServiceProvider
			)
		{

		}
		public AppContext(IServiceProvider sp) : base(sp)
		{

		}
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}