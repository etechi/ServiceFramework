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
using System.Net.Http;
using System.Reflection;
using SF.Sys.Reflection;
using SF.Sys.Serialization;
using SF.Sys.Metadata;
using SF.Sys.Auth;
using SF.Sys.Entities.Annotations;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Comments;
using SF.Sys.Annotations;

namespace SF.Sys.NetworkService
{
	public class ServiceMetadataBuilder : BaseMetadataBuilder
    {
        List<Metadata.Service> Services { get; } = new List<Metadata.Service>();
		IServiceBuildRuleProvider BuildRule { get; }
		public ServiceMetadataBuilder(
			IServiceBuildRuleProvider BuildRule, 
			IJsonSerializer JsonSerializer,
			IMetadataTypeCollection Types=null):
			base(JsonSerializer,Types)
		{
			this.BuildRule = BuildRule;
		}
		public Metadata.Library BuildLibrary(IEnumerable<Type> ServiceTypes)
        {
            Services.AddRange(
				ServiceTypes
                .Select(type => GenerateServiceMetadata(type))
                );
            return new Metadata.Library
			{
                Services = Services.ToArray(),
                Types = TypeCollection.GetTypes().ToArray()
            };
        }

		Metadata.Service GenerateServiceMetadata(Type type)
		{
			var attrs = type.AllInterfaces().SelectMany(i=>i.GetCustomAttributes()).Distinct();
			var aas = attrs.Where(a => a is AuthorizeAttribute).Cast<AuthorizeAttribute>().ToArray();
			var rps = attrs.Where(a => a is RequirePermissionAttribute).Cast<RequirePermissionAttribute>().ToArray();
			var roles = aas.Length == 0 ? null :
				aas.Where(aa => !string.IsNullOrWhiteSpace(aa.Roles))
				.SelectMany(aa => aa.Roles.Split(','))
				.Select(r => r.Trim())
				.Distinct()
				.ToArray();
            var ac = new Metadata.GrantInfo
            {
                UserRequired = aas.Length > 0,
                RolesRequired = roles,
                PermissionsRequired = rps.Length > 0 ? rps.Select(rp => rp.Resource + ":" + rp.Operation).ToArray() : null
            };

            return LoadAttributes(
				new Metadata.Service(type)
				{
					Name = BuildRule.FormatServiceName( type),
					Methods = GenerateMethodsMetadata(type, ac),
					GrantInfo= UseOrIgnoreGrantInfo(ac)
				},
				attrs,
				type,
				type.Comment(false)
				);

		}

		
		
		Metadata.Method[] GenerateMethodsMetadata(Type type, Metadata.GrantInfo ac)
		{
			var re = new List<Metadata.Method>();
			foreach (var m in BuildRule.GetServiceMethods(type)
				.Where(m => IsActionMethod(m)))
			{
				var a = GenerateMethodMetadata(m,ac);
				if (re.Any(r => r.Name == a.Name))
					continue;
				re.Add(a);

				//.Select(m => GenerateActionMetadata(m))
				//.ToArray();
			}
			return re.ToArray();
		}
		//static Type[] HttpMethodAttributes { get; } = new[]
		//{
		//	typeof(HttpGetAttribute),
		//	typeof(HttpPostAttribute),
		//	typeof(HttpPutAttribute),
		//	typeof(HttpDeleteAttribute),
		//	typeof(HttpPatchAttribute)
		//};
		bool IsActionMethod(MethodInfo method)
		{
			var cas=method.GetCustomAttributes();
			if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
				return false;
			//if (cas.All(a => !HttpMethodAttributes.Contains(a.GetType())))
			//	return false;
			if (method.GetCustomAttribute<IgnoreAttribute>(true) != null)
				return false;
			return true;
		}
		static Metadata.GrantInfo UseOrIgnoreGrantInfo(Metadata.GrantInfo ac)
		{
			if (!ac.UserRequired && (ac.RolesRequired == null || ac.RolesRequired.Length == 0) && (ac.PermissionsRequired == null || ac.PermissionsRequired.Length == 0))
				return null;
			return ac;
		}
		Metadata.Method GenerateMethodMetadata(MethodInfo method, Metadata.GrantInfo ac)
		{
			var attrs = method.GetCustomAttributes();
			var aas = attrs.Where(a => a is AuthorizeAttribute).Cast<AuthorizeAttribute>().ToArray();
			var rps = attrs.Where(a => a is RequirePermissionAttribute).Cast<RequirePermissionAttribute>().ToArray();
			var roles = aas.Length == 0 ? null : 
				aas.Where(aa=>!string.IsNullOrWhiteSpace(aa.Roles))
				.SelectMany(aa => aa.Roles.Split(','))
				.Select(r => r.Trim())
				.Distinct()
				.ToArray();
			var parameters= GenerateMethodParameters(method);
			return LoadAttributes(
				new Metadata.Method(method)
				{
					Name = BuildRule.FormatMethodName(method),
					Parameters = parameters.Length == 0 ? null : parameters,
					Type = ResolveResultType(method.ReturnType),
					GrantInfo = UseOrIgnoreGrantInfo(new Metadata.GrantInfo
					{
						UserRequired = aas.Length > 0 || ac.UserRequired,
						RolesRequired = roles != null && roles.Length > 0 ? roles : ac.RolesRequired,
						PermissionsRequired = rps != null && rps.Length > 0 ? rps.Select(rp => rp.Resource + ":" + rp.Operation).ToArray() : ac.PermissionsRequired
					}),
					HeavyParameter = BuildRule.DetectHeavyParameter(method)?.Name
				},
				attrs,
				null,
				method.Comment(false),
				null//a=>!HttpMethodAttributes.Contains(a.GetType())
				);
		}
		new Metadata.Parameter[] GenerateMethodParameters(MethodInfo method)
		{
			return BuildRule.GetMethodParameters(method).Select(p => GenerateParameter(method, p)).ToArray();
		}
		Metadata.Parameter GenerateParameter(MethodInfo method, ParameterInfo parameter)
		{
			var param_type = parameter.ParameterType;
			bool optional = parameter.IsOptional;
			var attrs = parameter.GetCustomAttributes();
			if (param_type.IsGeneric() && param_type.GetGenericTypeDefinition() == typeof(Nullable<>))
				param_type = param_type.GetGenericArguments()[0];
			if (attrs.Any(a => a is RequiredAttribute))
				optional = false; 



			return LoadAttributes(
				new Metadata.Parameter
				{
					Name = parameter.Name,
					Optional=optional,
					//TransportMode=attrs.Any(a=>a is FromBodyAttribute),
					Type = ResolveType(param_type)
				},
				attrs,
				parameter,
				parameter.Comment()
				/*,a=>!(a is FromBodyAttribute)*/
				);
		}
		public SF.Sys.Metadata.Models.Type GetUnknownType()
		{
			var re = TypeCollection.FindType("unknown");
			if (re == null)
			{
				re = new SF.Sys.Metadata.Models.Type
				{
					Name = "unknown"
				};
				TypeCollection.AddType(re);
			}
			return re;
		}
		public override SF.Sys.Metadata.Models.Type GenerateAndAddType(System.Type type)
		{
			if (type == typeof(HttpResponseMessage))
				return GetUnknownType();
			return base.GenerateAndAddType(type);
		}
		
		
		static Type[] IgnoreAttributeTypes { get; } = BaseMetadataBuilder.DefaultIgnoreAttributeTypes.Concat(
			new[]{
				typeof(AuthorizeAttribute),
				typeof(RequirePermissionAttribute),
                //typeof(CommentAttribute)
            }).ToArray();

		protected override Type[] GetIgnoreAttributeTypes()
		{
			return IgnoreAttributeTypes;
		}


		public override T LoadAttributes<T>(T item,IEnumerable<Attribute> attrs,object attrSource,Comment comment,Predicate<Attribute> predicate=null)
		{
			if (comment != null)
			{
				if (!string.IsNullOrWhiteSpace(comment.Title)) item.Title = comment.Title;
				if (!string.IsNullOrWhiteSpace(comment.Summary)) item.Description = comment.Summary;
				if (!string.IsNullOrWhiteSpace(comment.Group)) item.Group = comment.Group;
			}
			return base.LoadAttributes(item, attrs, attrSource, comment, predicate);
		}
		protected override IMetadataAttributeValuesProvider TryGetAttributeValuesProvider(Attribute attr)
		{
			return BuildRule.TryGetAttributeValuesProvider(attr);
		}
		
	}

	

}
