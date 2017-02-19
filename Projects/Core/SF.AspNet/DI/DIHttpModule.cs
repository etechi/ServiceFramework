using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using System.Web;
using SF.Core.DI;

namespace SF.AspNet.DI
{
	public class DIHttpModule :
		IHttpModule
	{
		public static IServiceProvider ServiceProvider { get; internal set; }
		public void Dispose()
		{
			
		}
		static object DIScopeKey { get; } = new object();
		internal static IServiceProvider GetServiceProvider()
		{
			var re = (IDIScope)HttpContext.Current.Items[DIScopeKey];
			if (re == null)
			{
				if (ServiceProvider == null)
					throw new NotSupportedException();
				HttpContext.Current.Items[DIScopeKey] = re = ServiceProvider.Resolve<IDIScopeFactory>().CreateScope();
			}

			return re.ServiceProvider;
		}
		public void Init(System.Web.HttpApplication context)
		{
			context.EndRequest += new EventHandler(this.OnEndRequest);
		}

		protected virtual void OnEndRequest(object sender, EventArgs e)
		{
			var sm = HttpContext.Current.Items[DIScopeKey] as IDIScope;
			if (sm != null)
				sm.Dispose();
		}
		public static void Register()
		{
			Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(DIHttpModule));
		}
	}
	public static class HttpContextExtension
	{
		public static IServiceProvider GetServiceProvider(this HttpContext context)
		{
			return DIHttpModule.GetServiceProvider();
		}
	}
}
