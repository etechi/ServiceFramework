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

using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Dispatcher;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using SF.Core.NetworkService;
using SF.Core.ServiceManagement;

namespace SF.AspNet.NetworkService
{	
	partial class ServiceActionDescriptor : HttpActionDescriptor
	{
		ReflectedHttpActionDescriptor DefaultActionDescriptor { get; }
		public override string ActionName { get; }
		Lazy<ActionExecutor> ActionExecutorInstance { get; }
		public override Collection<HttpMethod> SupportedHttpMethods { get; }
		public ParameterInfo HeavyParameter { get; }
		public Collection<HttpParameterDescriptor> Parameters { get; }
		
		bool IsHttpResponseMessage { get; }

		public ServiceActionDescriptor(
			HttpControllerDescriptor ControllerDescriptor,
			string ActionName,
			Collection<HttpMethod> SupportedHttpMethods,
			MethodInfo Method,
			 ParameterInfo HeavyParameter
			):base(ControllerDescriptor)
		{
			DefaultActionDescriptor = new ReflectedHttpActionDescriptor(ControllerDescriptor, Method);
			this.ActionName = ActionName;
			this.SupportedHttpMethods = SupportedHttpMethods;
			this.ActionExecutorInstance = new Lazy<ActionExecutor>(() => new ActionExecutor(Method));
			this.HeavyParameter = HeavyParameter;
			this.Parameters = new Collection<HttpParameterDescriptor>(
				Method.GetParameters()
				.Where(p=>p.ParameterType!=typeof(Entities.Paging))
				.Select(p => new ParameterDescriptor(
					this,
					p,
					p == HeavyParameter
					)).ToArray()
					);

			ReturnType = DefaultActionDescriptor.ReturnType;
			if (ReturnType != null)
			{
				if (ReturnType == typeof(HttpResponseMessage))
				{
					IsHttpResponseMessage = true;
					ReturnType = typeof(System.Web.Http.Results.ResponseMessageResult);
				}
				else if (ReturnType.GetGenericArgumentTypeAsTask() == typeof(HttpResponseMessage))
				{
					ReturnType = typeof(Task<System.Web.Http.Results.ResponseMessageResult>);
					IsHttpResponseMessage = true;
				}
			}
		}
		//internal sealed class FilterInfoComparer : IComparer<FilterInfo>
		//{
		//	private static readonly FilterInfoComparer _instance = new FilterInfoComparer();

		//	public static FilterInfoComparer Instance
		//	{
		//		get
		//		{
		//			return FilterInfoComparer._instance;
		//		}
		//	}

		//	public int Compare(FilterInfo x, FilterInfo y)
		//	{
		//		if (x == null && y == null)
		//		{
		//			return 0;
		//		}
		//		if (x == null)
		//		{
		//			return -1;
		//		}
		//		if (y == null)
		//		{
		//			return 1;
		//		}
		//		return x.Scope - y.Scope;
		//	}
		//}
		//public override Collection<FilterInfo> GetFilterPipeline()
		//{
		//	IEnumerable<IFilterProvider> filterProviders = this.Configuration.Services.GetFilterProviders();
		//	IEnumerable<FilterInfo> source = filterProviders
		//		.SelectMany(fp => fp.GetFilters(this.Configuration, this))
		//		.OrderBy((FilterInfo f) => f, FilterInfoComparer.Instance);
		//	//source = RemoveDuplicates(source.Reverse<FilterInfo>()).Reverse<FilterInfo>();
		//	return new Collection<FilterInfo>(source.ToList<FilterInfo>());
		//	//return base.GetFilterPipeline();
		//}
		public override Type ReturnType { get; }
		static bool TypeAllowsNullValue(Type type)
		{
			return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
		}

		private object ExtractParameterFromDictionary(ParameterInfo parameterInfo, IDictionary<string, object> parameters, HttpControllerContext controllerContext)
		{
			object obj;
			if (!parameters.TryGetValue(parameterInfo.Name, out obj))
			{
				throw new HttpResponseException(
					controllerContext.Request.CreateErrorResponse(
						HttpStatusCode.BadRequest,
						$"Can Not Found Argument: Controller:{controllerContext.Controller.GetType()} Action:{ActionName} Parameter:{parameterInfo.Name}"
						));
			}
			if (obj == null && !TypeAllowsNullValue(parameterInfo.ParameterType))
			{
				throw new HttpResponseException(
					controllerContext.Request.CreateErrorResponse(
						HttpStatusCode.BadRequest,
						$"Null Parameter Not Allowed: Controller:{controllerContext.Controller.GetType()} Action:{ActionName} Parameter:{parameterInfo.Name}"
						));
			}
			if (obj != null && !parameterInfo.ParameterType.IsInstanceOfType(obj))
			{
				throw new HttpResponseException(
					controllerContext.Request.CreateErrorResponse(
						HttpStatusCode.BadRequest,
						$"Parameter Type Error: Controller:{controllerContext.Controller.GetType()} Action:{ActionName} Parameter:{parameterInfo.Name}"
						));
			}
			return obj;
		}
		private object[] PrepareParameters(IDictionary<string, object> parameters, HttpControllerContext controllerContext)
		{
			var parameterInfos = DefaultActionDescriptor.MethodInfo.GetParameters();
			int num = parameterInfos.Length;
			object[] array = new object[num];
			for (int i = 0; i < num; i++)
			{
				if (parameterInfos[i].ParameterType == typeof(Entities.Paging))
					array[i] = Entities.Paging.Create(controllerContext.Request.GetQueryNameValuePairs(),40);
				else
					array[i] = this.ExtractParameterFromDictionary(parameterInfos[i], parameters, controllerContext);
			}
			return array;
		}
		Task<object> ExecuteAsyncInner(HttpControllerContext controllerContext, IDictionary<string, object> arguments, CancellationToken cancellationToken)
		{
			var ctrl = controllerContext.Controller as ServiceController;
			if (ctrl == null)
				return DefaultActionDescriptor.ExecuteAsync(controllerContext, arguments, cancellationToken);

			if (controllerContext == null)
			{
				throw new ArgumentException("controllerContext");
			}
			if (arguments == null)
			{
				throw new ArgumentException("arguments");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return TaskHelpers.Canceled<object>();
			}

			((ControllerSource)controllerContext.Request.GetDependencyScope().GetService(typeof(ControllerSource))).Controller = ctrl;


			Task<object> result;
			try
			{
				var arguments2 = this.PrepareParameters(arguments, controllerContext);
				var resolver= (IServiceResolver)controllerContext.Request.GetDependencyScope().GetService(typeof(IServiceResolver));
				arguments.TryGetValue("service", out var service);
				object inst;
				if (service == null || !(service is string))
					inst = resolver.ResolveServiceByType(null, ctrl.ControllerType, null);
				else if (long.TryParse((string)service, out var sid))
					inst = resolver.ResolveServiceByIdent(
						sid,
						ctrl.ControllerType
						);
				else
					inst = resolver.ResolveServiceByType(null, ctrl.ControllerType, (string)service);
				if (inst == null)
					result = Task.FromResult((object)new NotFoundResult(ctrl));
				else
					result = ActionExecutorInstance.Value.Execute(inst, arguments2);
			}
			catch (Exception exception)
			{
				result = TaskHelpers.FromError<object>(exception);
			}
			return result;
		}
		public override async Task<object> ExecuteAsync(HttpControllerContext controllerContext, IDictionary<string, object> arguments, CancellationToken cancellationToken)
		{
			var re = await ExecuteAsyncInner(controllerContext, arguments, cancellationToken);
			if (IsHttpResponseMessage)
				return new ResponseMessageResult((HttpResponseMessage)re);
			return re;
		}

		public override Collection<HttpParameterDescriptor> GetParameters()
		{
			return this.Parameters;
		}
	}
	
}
