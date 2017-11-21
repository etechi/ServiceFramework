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

using SF.Sys.CallPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF.Sys.Services
{
	
	public static class CallPlansDIServiceCollectionExtension
	{
		class CallableDefination : ICallableDefination
		{
			public Func<IServiceProvider,long?, ICallable> CallableCreator{get;set;}
			public string Type { get; set; }
		}

		public static void AddCallable<T>(this IServiceCollection sc)
			where T: class,ICallable
		{
			sc.AddScoped<T, T>();
			sc.AddSingleton<ICallableDefination>(sp => new CallableDefination
			{
				Type = typeof(T).FullName,
				CallableCreator = (isp,id) => (ICallable)isp.Resolve<T>()
			});
		}
		public static void AddServiceCallable<I>(this IServiceCollection sc)
			where I : class
		{
			sc.AddSingleton<ICallableDefination>(sp => {
				var svcResolver = sp.Resolve<IServiceDeclarationTypeResolver>();
				return new CallableDefination
				{
					Type = svcResolver.GetTypeIdent(typeof(I)),
					CallableCreator = (isp, id) =>
						(ICallable)isp.Resolve<I>(id.Value)
				};
			});
		}
	}
}
