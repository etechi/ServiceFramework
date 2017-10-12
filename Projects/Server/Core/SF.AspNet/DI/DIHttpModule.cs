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
			var re = (IServiceScope)HttpContext.Current.Items[DIScopeKey];
			if (re == null)
			{
				if (ServiceProvider == null)
					throw new NotSupportedException();
				HttpContext.Current.Items[DIScopeKey] = re = ServiceProvider.Resolve<IServiceScopeFactory>().CreateServiceScope();
			}

			return re.ServiceProvider;
		}
		public void Init(System.Web.HttpApplication context)
		{
			context.EndRequest += new EventHandler(this.OnEndRequest);
		}

		protected virtual void OnEndRequest(object sender, EventArgs e)
		{
			var sm = HttpContext.Current.Items[DIScopeKey] as IServiceScope;
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
