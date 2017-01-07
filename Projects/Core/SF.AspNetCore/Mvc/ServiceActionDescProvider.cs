using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SF.Reflection;
using SF.Services.Metadata;

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
		public void OnProvidersExecuted(ActionDescriptorProviderContext context)
		{
			foreach (var type in ServiceTypes)
			{
				var controllerName = GetControllerName(type);
				foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod))
				{
					var args = method.GetParameters();
					var heavyParameter = ServiceBuildRule.DetectHeavyParameter(method);
					context.Results.Add(new ControllerActionDescriptor
					{
						ActionName = method.Name,
						ControllerName = controllerName,
						ControllerTypeInfo = type.GetTypeInfo(),
						MethodInfo = method,
						AttributeRouteInfo = new Microsoft.AspNetCore.Mvc.Routing.AttributeRouteInfo
						{
							Template = $"{RoutePrefix}/{controllerName}/{method.Name}",
						},
						ActionConstraints = new List<IActionConstraintMetadata>
						{
							new Microsoft.AspNetCore.Mvc.Internal.HttpMethodActionConstraint(
								new[]
								{
									heavyParameter==null?"GET":"POST"
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
								BindingInfo= heavyParameter==a?new Microsoft.AspNetCore.Mvc.ModelBinding.BindingInfo
								{
									BindingSource=Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource.Body
								}:null
							}
							).Cast<ParameterDescriptor>().ToList(),
						DisplayName = $"{controllerName}/{method.Name}",
						RouteValues = new Dictionary<string, string>
							{
								{"controller", controllerName},
								{"action",method.Name}
							},
					});
				}
			}
		}

		public void OnProvidersExecuting(ActionDescriptorProviderContext context)
		{
		}
	}
}
