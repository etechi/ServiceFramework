using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SF.Services.NetworkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SF.AspNetCore.Mvc
{
	class ServiceActionDescProvider : IActionDescriptorProvider
	{
		public int Order => 0;
		public Type[] ServiceTypes { get; }
		public string RoutePrefix { get; }
		public IServiceBuildRuleProvider ServiceBuildRule { get; }
		public ServiceActionDescProvider(string RoutePrefix,IEnumerable<Type> ServiceTypes, IServiceBuildRuleProvider ServiceBuildRule)
		{
			this.RoutePrefix = RoutePrefix;
			this.ServiceTypes = ServiceTypes.ToArray();
			this.ServiceBuildRule = ServiceBuildRule;
		}
		string GetControllerName(Type Type)
		{
			return ServiceBuildRule.FormatServiceName(Type);
		}
		ControllerActionDescriptor BuildDescriptor(
			Type type, 
			string controllerName, 
			string ActionName, 
			string HttpMethod,
			ParameterInfo HeavyParameter,
			MethodInfo method
			)
		{
			var args = method.GetParameters();
			return new ControllerActionDescriptor
			{
				ActionName = ActionName,
				ControllerName = controllerName,
				ControllerTypeInfo = type.GetTypeInfo(),
				MethodInfo = method,
				AttributeRouteInfo = new Microsoft.AspNetCore.Mvc.Routing.AttributeRouteInfo
				{
					Template = $"{RoutePrefix}/{controllerName}/{ActionName}",
				},
				ActionConstraints = new List<IActionConstraintMetadata>
						{
							new Microsoft.AspNetCore.Mvc.Internal.HttpMethodActionConstraint(
								new[]
								{
									HttpMethod
								}
								)
						},
				BoundProperties = new List<ParameterDescriptor>(),
				Properties = new Dictionary<object, object>(),
				FilterDescriptors = new List<FilterDescriptor>(),
				Parameters = args.Select(
					a => new ControllerParameterDescriptor
					{
						Name = a.Name,
						ParameterType = a.ParameterType,
						ParameterInfo = a,
						BindingInfo = HeavyParameter == a ? new Microsoft.AspNetCore.Mvc.ModelBinding.BindingInfo
						{
							BindingSource = Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource.Body
						} : null
					}
					).Cast<ParameterDescriptor>().ToList(),
				DisplayName = $"{controllerName}/{ActionName}",
				RouteValues = new Dictionary<string, string>
							{
								{"controller", controllerName},
								{"action",ActionName}
							},
			};
		}
		ControllerActionDescriptor BuildDescriptorByMethod(Type type,string controllerName,MethodInfo method)
		{
			var heavyParameter = ServiceBuildRule.DetectHeavyParameter(method);
			return BuildDescriptor(
					type,
					controllerName,
					method.Name,
					heavyParameter == null ? "GET" : "POST",
					heavyParameter,
					method
					);
			
		}
		IEnumerable<ControllerActionDescriptor> BuildDescriptorByProperty(Type type, string controllerName, PropertyInfo prop)
		{
			if (prop.CanWrite)
			{
				var writeMethod = prop.GetSetMethod();
				var heavyParameter = ServiceBuildRule.DetectHeavyParameter(writeMethod);
				yield return BuildDescriptor(
						type,
						controllerName,
						prop.Name,
						"PUT",
						heavyParameter,
						writeMethod
						);
			}
			if (prop.CanRead)
			{
				var readMethod = prop.GetGetMethod();
				yield return BuildDescriptor(
						type,
						controllerName,
						prop.Name,
						"GET",
						null,
						readMethod
						);
			}

		}
		public void OnProvidersExecuted(ActionDescriptorProviderContext context)
		{
			foreach (var type in ServiceTypes)
			{
				var controllerName = GetControllerName(type);
				foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod))
				{
					context.Results.Add(BuildDescriptorByMethod(type, controllerName, method));
				}
				foreach(var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty))
				{
					foreach (var desc in BuildDescriptorByProperty(type, controllerName, prop))
						context.Results.Add(desc);
				}
			}
		}

		public void OnProvidersExecuting(ActionDescriptorProviderContext context)
		{
		}
	}
}
