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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Services
{
	public interface IServiceInvoker
	{
		IServiceDeclaration ServiceDeclaration { get; }
		MethodInfo Method { get; }
		object Invoke(IServiceProvider ServiceProvider, string Arguments);
		object Invoke(IServiceProvider ServiceProvider, object[] Arguments);
	}
	public interface IServiceInvokerProvider
	{
		IServiceInvoker Resolve(Type Service, MethodInfo Method);
	}
	public static class ServiceInvokerExtension
	{
		public static Task<object> InvokeAsync(
			this IServiceInvoker ServiceInvoker,
			IServiceProvider ServiceProvider,
			string Arguments
			)
		{
			var re = ServiceInvoker.Invoke(ServiceProvider, Arguments);
			if (re is Task task)
				return task.CastToObject();
			else
				return Task.FromResult(re);
		}
		public static Task<object> InvokeAsync(
			this IServiceInvoker ServiceInvoker,
			IServiceProvider ServiceProvider,
			object[] Arguments
			)
		{
			var re = ServiceInvoker.Invoke(ServiceProvider, Arguments);
			if (re is Task task)
				return task.CastToObject();
			else
				return Task.FromResult(re);
		}
	}
}
