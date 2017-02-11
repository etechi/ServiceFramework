using SF.Core.DI;
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
using SF.Core.NetworkService;

namespace SF.AspNet.NetworkService
{
	class ServiceHttpActionSelector : IHttpActionSelector
	{
		ApiControllerActionSelector DefaultSelector { get; } = new ApiControllerActionSelector();
		class ActionDescriptors
		{
			public ILookup<string, HttpActionDescriptor> Descriptors{get;set ;}
		}
		Dictionary<Type, ActionDescriptors> ServiceTypes { get; }
		public IServiceBuildRuleProvider ServiceBuildRule { get; }

		ServiceActionDescriptor BuildDescriptor(
			HttpControllerDescriptor ControllerDescriptor,
			string ActionName,
			HttpMethod HttpMethod,
			ParameterInfo HeavyParameter,
			MethodInfo method
			)
		{
			return new ServiceActionDescriptor(
				ControllerDescriptor,
				ActionName,
				new Collection<HttpMethod>(new List<HttpMethod> { HttpMethod }),
				method,
				HeavyParameter
				);
		}
		ServiceActionDescriptor BuildDescriptorByMethod(HttpControllerDescriptor ControllerDescriptor, MethodInfo method)
		{
			var heavyParameter = ServiceBuildRule.DetectHeavyParameter(method);
			return BuildDescriptor(
					ControllerDescriptor,
					ServiceBuildRule.FormatMethodName(method),
					heavyParameter == null && !method.IsDefined(typeof(HeavyMethodAttribute),true) ? HttpMethod.Get:HttpMethod.Post,
					heavyParameter,
					method
					);

		}
		IEnumerable<ServiceActionDescriptor> BuildDescriptorByProperty(HttpControllerDescriptor ControllerDescriptor, PropertyInfo prop)
		{
			if (prop.CanWrite)
			{
				var writeMethod = prop.GetSetMethod();
				var heavyParameter = ServiceBuildRule.DetectHeavyParameter(writeMethod);
				yield return BuildDescriptor(
						ControllerDescriptor,
						prop.Name,
						HttpMethod.Put,
						heavyParameter,
						writeMethod
						);
			}
			if (prop.CanRead)
			{
				var readMethod = prop.GetGetMethod();
				yield return BuildDescriptor(
						ControllerDescriptor,
						prop.Name,
						HttpMethod.Get,
						null,
						readMethod
						);
			}

		}
		

		ILookup<string,HttpActionDescriptor> BuildActionDescriptors(HttpControllerDescriptor controllerDescriptor)
		{
			var type = controllerDescriptor.ControllerType;
			var descriptors = new List<HttpActionDescriptor>();
			foreach (var method in ServiceBuildRule.GetServiceMethods(type))
			{
				descriptors.Add(BuildDescriptorByMethod(controllerDescriptor, method));
			}
			foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty))
			{
				foreach (var desc in BuildDescriptorByProperty(controllerDescriptor, prop))
					descriptors.Add(desc);
			}
			return descriptors.ToLookup(d => d.ActionName,StringComparer.CurrentCultureIgnoreCase);
		}

		public ServiceHttpActionSelector(IServiceBuildRuleProvider ServiceBuildRule,Type[] ServiceTypes)
		{
			this.ServiceBuildRule = ServiceBuildRule;
			this.ServiceTypes = ServiceTypes.ToDictionary(
				t => t, 
				t => new ActionDescriptors()
				);
		}
		ILookup<string, HttpActionDescriptor> GetActionDescriptors(HttpControllerDescriptor controllerDescriptor) {
			ActionDescriptors descs;
			if (!ServiceTypes.TryGetValue(controllerDescriptor.ControllerType, out descs))
				return null;
			if (descs.Descriptors != null)
				return descs.Descriptors;
			lock (descs)
			{
				if (descs.Descriptors != null)
					return descs.Descriptors;
				descs.Descriptors = BuildActionDescriptors(controllerDescriptor);
			}
			return descs.Descriptors;
		}
		public ILookup<string, HttpActionDescriptor> GetActionMapping(HttpControllerDescriptor controllerDescriptor)
		{
			var lookup = GetActionDescriptors(controllerDescriptor);
			if (lookup != null)
				return lookup;
			return DefaultSelector.GetActionMapping(controllerDescriptor);
		}

		public HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
		{
			var lookup = GetActionDescriptors(controllerContext.ControllerDescriptor);
			if (lookup != null)
				return lookup[(string)controllerContext.RouteData.Values["action"]].First();

			return DefaultSelector.SelectAction(controllerContext);
		}
	}
	

}
