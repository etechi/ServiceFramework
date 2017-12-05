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

using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SF.Sys.NetworkService;
using SF.Sys.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SF.Sys.AspNetCore.NetworkServices
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
		static List<FilterDescriptor> filters { get; }=new List<FilterDescriptor>
		{
			new FilterDescriptor(
				new NetworkServiceResultFilter(),
				0
				)

		};
		ControllerActionDescriptor BuildDescriptor(
			Type type, 
			string controllerName, 
			string ActionName, 
			string HttpMethod,
			bool HeavyMode,
			MethodInfo method
			)
		{
			var args = ServiceBuildRule.GetMethodParameters(method).ToArray();
			List<ParameterDescriptor> ps;
			if(!HeavyMode || args.Length <= 1)
			{
				ps = args.Select(
					a => new ControllerParameterDescriptor
					{
						Name = a.Name,
						ParameterType = a.ParameterType,
						ParameterInfo = a,
						BindingInfo = HeavyMode ? new Microsoft.AspNetCore.Mvc.ModelBinding.BindingInfo
						{
							BindingSource = Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource.Body
						} : null
					}
					).Cast<ParameterDescriptor>().ToList();
			}
			else
			{
				var tb = new DynamicTypeBuilder();
				var te = new TypeExpression(
						ActionName + "Arguments",
						null,
						TypeAttributes.Public);
				foreach (var a in args)
					te.Properties.Add(new PropertyExpression(a.Name,new SystemTypeReference( a.ParameterType), PropertyAttributes.None));

				var argType = tb.Build(new[] { te });
				ps = new List<ParameterDescriptor>
				{
					new ControllerParameterDescriptor
					{
						Name = a.Name,
						ParameterType = a.ParameterType,
						ParameterInfo = a,
						BindingInfo = HeavyMode ? new Microsoft.AspNetCore.Mvc.ModelBinding.BindingInfo
						{
							BindingSource = Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource.Body
						} : null
					}
				};
			}
			return new ControllerActionDescriptor
			{
				ActionName = ActionName,
				ControllerName = controllerName,
				ControllerTypeInfo = type.GetTypeInfo(),
				MethodInfo = method,
				//AttributeRouteInfo = new Microsoft.AspNetCore.Mvc.Routing.AttributeRouteInfo
				//{
				//	Template = $"{RoutePrefix}/{controllerName}/{ActionName}",
				//},
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
				FilterDescriptors = filters,
				Parameters = args.Select(
					a => new ControllerParameterDescriptor
					{
						Name = a.Name,
						ParameterType = a.ParameterType,
						ParameterInfo = a,
						BindingInfo = HeavyMode ? new Microsoft.AspNetCore.Mvc.ModelBinding.BindingInfo
						{
							BindingSource = method.GetParameters().Length==1 ? Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource.Body: Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource.Form
						} : null
					}
					).Cast<ParameterDescriptor>().ToList(),

				DisplayName = $"{controllerName}/{ActionName}",
				RouteValues = new Dictionary<string, string>
							{
								{"controller", controllerName},
								{"action",ActionName},
								{"scope","api" }
							},
			};
		}
		ControllerActionDescriptor BuildDescriptorByMethod(Type type,string controllerName,MethodInfo method)
		{
			var heavyMode = ServiceBuildRule.DetectHeavyMode(method);
			return BuildDescriptor(
					type,
					controllerName,
					ServiceBuildRule.FormatMethodName(method),
					!method.IsDefined(typeof(HeavyMethodAttribute)) && !heavyMode? "GET" : "POST",
					heavyMode,
					method
					);
			
		}
		IEnumerable<ControllerActionDescriptor> BuildDescriptorByProperty(Type type, string controllerName, PropertyInfo prop)
		{
			if (prop.CanWrite)
			{
				var writeMethod = prop.GetSetMethod();
				var heavyParameter = ServiceBuildRule.DetectHeavyMode(writeMethod);
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
						false,
						readMethod
						);
			}

		}
		public void OnProvidersExecuted(ActionDescriptorProviderContext context)
		{
			foreach (var type in ServiceTypes)
			{
				var controllerName = GetControllerName(type);
				foreach (var method in ServiceBuildRule.GetServiceMethods(type))
				{
					context.Results.Add(BuildDescriptorByMethod(type, controllerName, method));
				}
				foreach(var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.FlattenHierarchy))
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
