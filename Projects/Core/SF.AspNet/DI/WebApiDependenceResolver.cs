using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using System.Web;
using SF.Core.ServiceManagement;
namespace SF.AspNet.DI
{
	public class DependencyScope : System.Web.Http.Dependencies.IDependencyScope
	{
		public IServiceProvider ServiceProvider { get; }
		public DependencyScope(IServiceProvider ServiceProvider)
		{
			this.ServiceProvider = ServiceProvider;
		}
		public void Dispose()
		{
		}

		public object GetService(Type serviceType)
		{
			var re=ServiceProvider.GetService(serviceType);
			if(re==null)
				System.Diagnostics.Debugger.Log(1, "DI", $"Resolve Type {serviceType} Failed\n");
			return re;
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return ServiceProvider.GetServices(serviceType);
		}
	}
	public class WebApiDependenceResolver : System.Web.Http.Dependencies.IDependencyResolver
	{
		public IServiceProvider ServiceProvider { get; }
		public WebApiDependenceResolver(IServiceProvider ServiceProvider)
		{
			this.ServiceProvider = ServiceProvider;
		}

		public IDependencyScope BeginScope()
		{
			return new DependencyScope(DIHttpModule.GetServiceProvider());
		}

		public object GetService(Type serviceType)
		{
			return ServiceProvider.GetService(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return ServiceProvider.GetServices(serviceType);
		}

		public void Dispose()
		{
		}
	}

}
