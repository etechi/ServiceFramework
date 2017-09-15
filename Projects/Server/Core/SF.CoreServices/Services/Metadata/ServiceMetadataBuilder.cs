using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using SF.Annotations;
using SF.Reflection;
namespace SF.Services.Metadata
{
	public class ServiceMetadataBuilder : SF.Metadata.BaseMetadataBuilder
    {
        List<Models.Service> Services { get; } = new List<Models.Service>();
		IServiceBuildRuleProvider BuildRule { get; }
		public ServiceMetadataBuilder(IServiceBuildRuleProvider BuildRule, Serialization.IJsonSerializer JsonSerializer):
			base(JsonSerializer)
		{
			this.BuildRule = BuildRule;
		}
		public Models.Library BuildLibrary(IEnumerable<Type> ServiceTypes)
        {
            Services.AddRange(
				ServiceTypes
                .Select(type => GenerateServiceMetadata(type))
                );
            return new Models.Library
			{
                Services = Services.ToArray(),
                Types = GetTypes()
            };
        }

		Models.Service GenerateServiceMetadata(Type type)
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
            var ac = new Models.GrantInfo
            {
                UserRequired = aas.Length > 0,
                RolesRequired = roles,
                PermissionsRequired = rps.Length > 0 ? rps.Select(rp => rp.Resource + ":" + rp.Operation).ToArray() : null
            };

            return LoadAttributes(
				new Models.Service(type)
				{
					Name = BuildRule.FormatServiceName( type),
					Methods = GenerateMethodsMetadata(type, ac),
					GrantInfo= UseOrIgnoreGrantInfo(ac)
				},
				attrs
				);

		}
		Models.Method[] GenerateMethodsMetadata(Type type, Models.GrantInfo ac)
		{
			var re = new List<Models.Method>();
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
		static Models.GrantInfo UseOrIgnoreGrantInfo(Models.GrantInfo ac)
		{
			if (!ac.UserRequired && (ac.RolesRequired == null || ac.RolesRequired.Length == 0) && (ac.PermissionsRequired == null || ac.PermissionsRequired.Length == 0))
				return null;
			return ac;
		}
		Models.Method GenerateMethodMetadata(MethodInfo method, Models.GrantInfo ac)
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
				new Models.Method(method)
				{
					Name = method.Name,
					Parameters = parameters.Length==0? null : parameters,
					Type = ResolveResultType(method.ReturnType),
					GrantInfo= UseOrIgnoreGrantInfo(new Models.GrantInfo
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
		new Models.Parameter[] GenerateMethodParameters(MethodInfo method)
		{
			return method.GetParameters().Select(p => GenerateParameter(method, p)).ToArray();
		}
		Models.Parameter GenerateParameter(MethodInfo method, ParameterInfo parameter)
		{
			var param_type = parameter.ParameterType;
			bool optional = parameter.IsOptional;
			var attrs = parameter.GetCustomAttributes();
			if (param_type.IsGeneric() && param_type.GetGenericTypeDefinition() == typeof(Nullable<>))
				param_type = param_type.GetGenericArguments()[0];
			if (attrs.Any(a => a is RequiredAttribute))
				optional = false; 

			return LoadAttributes(new Models.Parameter
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
