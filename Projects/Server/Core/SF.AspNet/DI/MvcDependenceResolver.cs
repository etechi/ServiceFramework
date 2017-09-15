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

	public class MvcDependenceResolver : System.Web.Mvc.IDependencyResolver
	{
		public object GetService(Type serviceType)
		{
			return DIHttpModule.GetServiceProvider().GetService(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return DIHttpModule.GetServiceProvider().GetServices(serviceType);
		}
	}

}
