using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using SF.Core.Serialization;
using SF.Auth;
using SF.Metadata;

namespace SF.Services.NetworkService
{
	public class ServiceMetadataBuilder : SF.Metadata.BaseMetadataBuilder
    {
        List<Metadata.Service> Services { get; } = new List<Metadata.Service>();
		IServiceBuildRuleProvider BuildRule { get; }
		public ServiceMetadataBuilder(IServiceBuildRuleProvider BuildRule, IJsonSerializer JsonSerializer):
			base(JsonSerializer)
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
                Types = GetTypes()
            };
        }

		Metadata.Service GenerateServiceMetadata(Type type)
		{
			var attrs = type.GetCustomAttributes(true).Cast<Attribute>();
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
				attrs
				);

		}
		Metadata.Method[] GenerateMethodsMetadata(Type type, Metadata.GrantInfo ac)
		{
			var re = new List<Metadata.Method>();
			foreach (var m in type.
				GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod)
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
					Name = method.Name,
					Parameters = parameters.Length==0? null : parameters,
					Type = ResolveResultType(method.ReturnType),
					GrantInfo= UseOrIgnoreGrantInfo(new Metadata.GrantInfo
					{
						UserRequired = aas.Length > 0 || ac.UserRequired,
						RolesRequired = roles != null && roles.Length > 0 ? roles : ac.RolesRequired,
						PermissionsRequired = rps != null && rps.Length > 0 ? rps.Select(rp => rp.Resource + ":" + rp.Operation).ToArray() : ac.PermissionsRequired
					}),
					HeavyParameter = BuildRule.DetectHeavyParameter(method)?.Name
				},
				attrs,
				null//a=>!HttpMethodAttributes.Contains(a.GetType())
				);
		}
		new Metadata.Parameter[] GenerateMethodParameters(MethodInfo method)
		{
			return method.GetParameters().Select(p => GenerateParameter(method, p)).ToArray();
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

			return LoadAttributes(new Metadata.Parameter
			{
				Name = parameter.Name,
				Optional=optional,
				//TransportMode=attrs.Any(a=>a is FromBodyAttribute),
				Type = ResolveType(param_type)
			},attrs/*,a=>!(a is FromBodyAttribute)*/);
		}
	
		protected override SF.Metadata.Models.Type GenerateType(System.Type type)
		{
			if (type == typeof(HttpResponseMessage))
			{
				var re = new SF.Metadata.Models.Type
				{
					Name = "unknown"
				};
				AddType(type, re);
				return re;
			}
			return base.GenerateType(type);
		}
		
		
		static Type[] IgnoreAttributeTypes { get; } = SF.Metadata.BaseMetadataBuilder.DefaultIgnoreAttributeTypes.Concat(
			new[]{
				typeof(AuthorizeAttribute),
				typeof(RequirePermissionAttribute),
                typeof(DisplayAttribute)
            }).ToArray();

		protected override Type[] GetIgnoreAttributeTypes()
		{
			return IgnoreAttributeTypes;
		}


		protected override T LoadAttributes<T>(T item,IEnumerable<Attribute> attrs,Predicate<Attribute> predicate=null)
		{
			var display= (DisplayAttribute)attrs.FirstOrDefault(a => a is DisplayAttribute);
			if (display != null)
			{
				if (!string.IsNullOrWhiteSpace(display.Name)) item.Title = display.Name;
				if (!string.IsNullOrWhiteSpace(display.Description)) item.Description = display.Description;
				if (!string.IsNullOrWhiteSpace(display.GroupName)) item.Group = display.GroupName;
			}
			return base.LoadAttributes(item, attrs, predicate);
		}
		
	}

	

}
