using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SF.Reflection;
using SF.Serialization;
namespace SF.Metadata
{
	public class BaseMetadataBuilder
	{
		Dictionary<Type, Models.Type> Types { get; } = new Dictionary<Type, Models.Type>();
		public Serialization.IJsonSerializer JsonSerializer { get; }
		public BaseMetadataBuilder(Serialization.IJsonSerializer JsonSerializer)
		{
			this.JsonSerializer = JsonSerializer;
		}

		public Models.Type[] GetTypes() { return Types.Values.ToArray(); }

		protected void AddType(Type Type,Models.Type ModelType)
		{
			Types.Add(Type, ModelType);
		}
		public virtual Models.Library Build()
		{
			return new Models.Library { Types = GetTypes() };
		}
		protected virtual Models.Parameter[] GenerateMethodParameters(MethodInfo method)
		{
			return method.GetParameters().Select(p => GenerateMethodParameter(method, p)).ToArray();
		}
		protected virtual Models.Parameter GenerateMethodParameter(MethodInfo method, ParameterInfo parameter)
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
				Type = ResolveType(param_type),
                DefaultValue= parameter.HasDefaultValue? JsonSerializer.Serialize(parameter.DefaultValue):null
            },attrs);
		}

		protected virtual string ResolveResultType(Type type)
		{
			if (type == typeof(Task))
				return ResolveType(typeof(void));
			if (type.IsGeneric() && type.GetGenericTypeDefinition() == typeof(Task<>))
				return ResolveType(type.GetGenericArguments()[0]);
			return ResolveType(type);
		}
		protected virtual string ResolveType(Type type)
		{
			return TryGenerateType(type).Name;
		}
		protected virtual Type TryDetectDictType(Type type)
		{
			if(type.IsGeneric() && type.GetGenericTypeDefinition()==typeof(IDictionary<,>))
				return type.GetGenericArguments()[1];

			var interfaces = type.GetInterfaces();
			var dict_type = interfaces
				.FirstOrDefault(t => t.IsGeneric() && t.GetGenericTypeDefinition() == typeof(IDictionary<,>));
			if (dict_type != null)
				return dict_type.GetGenericArguments()[1];

			return null;
		}

		public virtual Models.Type TryGenerateType(Type type)
		{
			Models.Type re;
			if (Types.TryGetValue(type, out re))
				return re;
			return GenerateType(type);
		}
		protected virtual Models.Type GenerateType(Type type)
		{ 
			var re = new Models.Type
			{
				Name = FormatTypeName(type),
				IsEnumType = type.IsEnumType(),
				IsInterface = type.IsInterfaceType()
			};
			Types.Add(type, re);

			if (type.IsArray)
			{
				re.ElementType = ResolveType(type.GetElementType());
				re.IsArrayType = true;
				return re;
			}

			if (type.IsGeneric())
			{
				var dict_type = TryDetectDictType(type);
				if (dict_type != null)
				{
					re.ElementType = ResolveType(dict_type);
					re.IsDictType = true;
					return re;
				}
				if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					re.ElementType = ResolveType(type.GetGenericArguments()[0]);
					re.IsArrayType = true;
					return re;
				}
			}
			if (type.IsEnumType())
			{
				re.Properties = type
					.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
					.Select(p =>
					{
						var da = p.GetCustomAttribute<DisplayAttribute>();
						var i= new Models.Property
						{
							Name = p.Name,
							Type = re.Name,
							Title=da?.Name,
							Description=da?.Description,
							Prompt=da?.Prompt,
							Group=da?.GroupName,
						};
                        LoadAttributes(i, p.GetCustomAttributes());
                        return i;
                    })
					.ToArray();
				return re;
			}
			 if (!IsRootType(type))
			{
				var baseTypes = new List<string>();
				if (!IsRootType(type.GetBaseType()))
					baseTypes.Add(ResolveType(type.GetBaseType()));
				if(type.IsInterfaceType())
				{
					foreach (var i in type.GetInterfaces())
						baseTypes.Add(ResolveType(i));
				}
				if (baseTypes.Count > 0)
					re.BaseTypes = baseTypes.ToArray();
				var props=GenerateTypeProperties(type);
				re.Properties = props.Length > 0 ? props : null;
				LoadAttributes(re, type.GetCustomAttributes(true).Cast<Attribute>());
			}
			
			return re;
		}
		protected virtual string FormatTypeName(Type type)
		{
			if (type.IsArray)
				return FormatTypeName(type.GetElementType()) + "[]";
			if (type.IsGeneric())
			{
				var dict_type = TryDetectDictType(type);
				if (dict_type != null)
					return FormatTypeName(dict_type) + "<>";


				if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
					return FormatTypeName(type.GetGenericArguments()[0]) + "[]";

				var tas=type.GetGenericArguments();
				var type_name = type.Name;
				var i = type_name.LastIndexOf('`');
				if(i!=-1)
					type_name=type_name.Substring(0, i);
				return type_name + "_" + string.Join("_",tas.Select(t => FormatTypeName(t)));
			}



			var name = type.FullName;
			switch (name)
			{
				case "System.Object": return "object";
				case "System.String": return "string";
				case "System.DateTime": return "datetime";
                case "System.TimeSpan": return "timespan";

                case "System.Int64": return "long";
				case "System.Int32": return "int";
				case "System.Int16": return "short";
				case "System.SByte": return "sbyte";

				case "System.UInt64": return "ulong";
				case "System.UInt32": return "uint";
				case "System.UInt16": return "ushort";
				case "System.Byte": return "byte";

				case "System.Single": return "float";
				case "System.Double": return "double";

				case "System.Char": return "char";
				case "System.Boolean": return "bool";
				case "System.Decimal": return "decimal";
				case "System.Void":return "void";
				default:
					return name;
			}
		}
		protected virtual Models.Property[] GenerateTypeProperties(Type type)
		{
            object obj = null;
            if (!type.IsAbstractType() && !type.IsInterfaceType() && !type.IsGenericDefinition())
            {
                var ci = type.GetConstructor(Array.Empty<Type>());
                if (ci != null)
                    obj = Activator.CreateInstance(type);
            }
			return type.GetProperties(
				BindingFlags.Public |
				BindingFlags.Instance
				)
				.Where(p=>p.DeclaringType== type)
				.Select(p => GenerateTypeProperty(type, p, obj))
				.ToArray();
		}

        static object TypeDefaultValue<T>(){
            return default(T);
        }
        static MethodInfo MethodGetTypeDefaultValue = typeof(BaseMetadataBuilder).GetMethod(
			"TypeDefaultValue",
            BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.NonPublic
            );
        static object GetTypeDefaultValue(Type type)
        {
            var m = MethodGetTypeDefaultValue.MakeGenericMethod(type);
            return m.Invoke(null,Array.Empty<object>());
        }
		protected virtual Models.Property GenerateTypeProperty(Type type,PropertyInfo prop,object DefaultValueObject)
		{
			var attrs = prop.GetCustomAttributes();
			var prop_type = prop.PropertyType;
            bool optional;
			if (prop_type.IsGeneric() && prop_type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				prop_type = prop_type.GetGenericArguments()[0];
                optional = true;
            }
            else
                optional = !prop_type.IsValue();

            if (prop_type == typeof(bool))
                optional = true;


            if (attrs.Any(a => a is SF.Annotations.OptionalAttribute /*|| a is ReadOnlyAttribute && ((ReadOnlyAttribute)a).IsReadOnly*/))
                optional = true;
            else if (attrs.Any(a => a is RequiredAttribute))
                optional = false;
            

            string def_value = null;
            if(DefaultValueObject!=null )
            {
                var v = prop.GetValue(DefaultValueObject);
                if (v != null && !v.Equals(GetTypeDefaultValue(prop.PropertyType)))
                    def_value = JsonSerializer.Serialize(v);
            }

			return LoadAttributes(new Models.Property
			{
				Name = prop.Name,
				Type = ResolveType(prop_type),
				Optional = optional,
                DefaultValue= def_value
            },attrs);
		}
		protected virtual bool IsRootType(Type type)
		{
			return type==null ||
				type==typeof(Array) ||
				type == typeof(string) ||
				type.IsValue() ||
				type == typeof(object) || 
				type==typeof(System.Enum)||
				//type == typeof(MarshalByRefObject) || 
				type == typeof(ValueType)
				;
		}

		public static Type[] DefaultIgnoreAttributeTypes { get; } = new[]
		{
			typeof(System.Runtime.CompilerServices.AsyncStateMachineAttribute),
			typeof(System.Diagnostics.DebuggerStepThroughAttribute),
			typeof(DisplayAttribute),
			typeof(System.ComponentModel.DataAnnotations.RequiredAttribute),
            typeof(System.Runtime.InteropServices.OptionalAttribute),
        };

		protected virtual Type[] GetIgnoreAttributeTypes()
		{
			return DefaultIgnoreAttributeTypes;
		}

		protected virtual T LoadAttributes<T>(T item,IEnumerable<Attribute> attrs,Predicate<Attribute> predicate=null)where T : Models.Entity
		{
			var display = (System.ComponentModel.DataAnnotations.DisplayAttribute)
				attrs.FirstOrDefault(a => a is System.ComponentModel.DataAnnotations.DisplayAttribute);
			if (display != null)
			{
				item.Title = display.Name;
				item.Description = display.Description;
				item.Group = display.GroupName;
				item.Prompt = display.Prompt;
				item.ShortName = display.ShortName;
			}
			
			var re = attrs
				.Where(a=>!GetIgnoreAttributeTypes().Contains(a.GetType()))
				.Where(a => (predicate==null || predicate(a)) && a.GetType().IsPublicType())
				.Select(a => GenerateAttribute(a)).ToArray();
			if(re.Length>0)
				item.Attributes=re;
			return item;
		}
		Models.Attribute GenerateAttribute(Attribute attr)
		{
			var type = attr.GetType();
			var values = JsonSerializer.Serialize(attr);
			return new Models.Attribute
			{
				Type = type.FullName,
				Values = values.Length > 0 && values!="{}" ? values : null
			};
		}
		
	}

	

}
