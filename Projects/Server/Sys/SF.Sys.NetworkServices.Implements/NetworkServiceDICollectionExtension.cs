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

using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using SF.Sys.Services;
using SF.Sys.NetworkService;
using SF.Sys.Serialization;
using SF.Sys.Reflection;
using System.Net;
using SF.Sys.Collections.Generic;

namespace SF.Sys.Services
{
	public static class NetworkServiceDICollectionExtension
	{
		class LocalInvokeContext : ILocalInvokeContext, IInvokeContext,ILocalInvokeRequest, ILocalInvokeResponse, IUploadedFileCollection
		{
			public IInvokeRequest Request => this;

			public IInvokeResponse Response => this;

			public string Method { get; set; } = "Direct";

			public string Uri { get; set; } = "local:";

			public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; } = new Dictionary<string, IEnumerable<string>>();

			public string Status { get; set; }

			public IUploadedFile[] Files => Array.Empty<IUploadedFile>();

			IDictionary<string, IEnumerable<string>> IInvokeResponse.Headers { get; } = new Dictionary<string, IEnumerable<string>>();

            IDictionary<string, IEnumerable<string>> ILocalInvokeRequest.Headers => throw new NotImplementedException();

            ILocalInvokeRequest ILocalInvokeContext.Request => this;
            ILocalInvokeResponse ILocalInvokeContext.Response=> this;
            Dictionary<string, string> _RequestCookies;

            string IInvokeRequest.GetCookie(string Key) => _RequestCookies?.Get(Key);
            void ILocalInvokeRequest.SetCookie(Cookie Cookie)
            {
                if (_RequestCookies == null)
                    _RequestCookies = new Dictionary<string, string>();
                _RequestCookies[Cookie.Name] = Cookie.Value;
            }


            Dictionary<string, Cookie> _ResponseCookies;
            void IInvokeResponse.SetCookie(Cookie Cookie)
            {
                if (_ResponseCookies == null)
                    _ResponseCookies = new Dictionary<string, Cookie>();
                _ResponseCookies[Cookie.Name] = Cookie;
            }
            Cookie ILocalInvokeResponse.GetCookie(string Key)
            {
                return _ResponseCookies?.Get(Key);
            }

        }
		public static IServiceCollection AddLocalInvokeContext(this IServiceCollection sc)
		{
			
            sc.AddScoped<ILocalInvokeContext, LocalInvokeContext>();
            sc.AddScoped<IInvokeContext>(sp=>(IInvokeContext)sp.Resolve<ILocalInvokeContext>());
            sc.AddScoped<IUploadedFileCollection, LocalInvokeContext>();


			return sc;
		}

		public static IServiceCollection AddNetworkService(
					this IServiceCollection sc,
					IEnumerable<Type> Services=null
					)
		{
			sc.AddScoped<IClientSettingService, ClientSettingService>();
			sc.AddSingleton<IServiceBuildRuleProvider, DefaultServiceBuildRuleProvider>();
			sc.AddSingleton(sp => {
				var builder = new ServiceMetadataBuilder(
					sp.Resolve<IServiceBuildRuleProvider>(),
					sp.Resolve<IJsonSerializer>()
					);
				foreach (var ests in sp.TryResolve<IEnumerable<IExtraServiceTypeSource>>())
					ests.AddExtraServiceType(builder);
				return builder.BuildLibrary(sp.Resolve<IServiceTypeCollection>().Types);
			});
			sc.AddScoped<IServiceMetadataService, DefaultServiceMetadataService>();
			sc.AddSingleton<IServiceTypeCollection>(sp =>
			{
				var ServiceTypes = (from svc in (Services ??
					sc.GetServiceTypes())
					where svc.AllInterfaces().Any(i=>i.GetCustomAttribute<NetworkServiceAttribute>() != null)
					select svc
					).ToArray();
				return new ServiceTypeCollection(ServiceTypes);
			});

			return sc;
		}
	}

}
