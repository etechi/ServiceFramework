#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
