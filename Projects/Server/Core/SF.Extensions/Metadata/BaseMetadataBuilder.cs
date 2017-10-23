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
using System.Threading.Tasks;
using System.Reflection;
using SF.Core.Serialization;
using System.ComponentModel.DataAnnotations;

namespace SF.Metadata
{
	public class MetadataTypeCollection : IMetadataTypeCollection
	{
		Dictionary<string, Models.Type> Types { get; } = new Dictionary<string, Models.Type>();

		public IEnumerable<Models.Type> GetTypes() { return Types.Values; }
		public Models.Type FindType(string Name)
		{
			return Types.Get(Name);
		}
		public void AddType(Models.Type ModelType)
		{
			Types.Add(ModelType.Name, ModelType);
		}
	}
	public class BaseMetadataBuilder : IMetadataBuilder
	{
		public IMetadataTypeCollection TypeCollection { get; }
		public IJsonSerializer JsonSerializer { get; }
		public BaseMetadataBuilder(IJsonSerializer JsonSerializer, IMetadataTypeCollection Types)
		{
			this.JsonSerializer = JsonSerializer;
			this.TypeCollection = Types ?? new MetadataTypeCollection();
		}

		public virtual Models.Library Build()
		{
			return new Models.Library { Types = TypeCollection.GetTypes().ToArray() };
		}
		protected virtual Models.Parameter[] GenerateMethodParameters(MethodInfo method)
		{
			return method.GetParameters().Select(p => GenerateMethodParameter(method, p)).ToArray();
		}
		protected virtual Models.Parameter GenerateMethodParameter(MethodInfo method, ParameterInfo parameter)
		{
			var param_type = parameter.ParameterType;
			bool optional = parameter.IsOptional || parameter.IsDefined(typeof(OptionalAttribute));
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
            },attrs, parameter);
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
			return TryGenerateAndAddType(type).Name;
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

		public virtual Models.Type TryGenerateAndAddType(Type type)
		{
			if (type.IsGeneric() && type.IsGenericTypeOf(typeof(Nullable<>)))
				type = type.GenericTypeArguments[0];

			var re = TypeCollection.FindType(FormatTypeName(type));
			if (re != null) return re;
			return GenerateAndAddType(type);
		}
		
		static bool IsGenericArrayType(Type type)
		{
			return type.IsGenericTypeOf(typeof(IEnumerable<>)) ||
				type.IsGenericTypeOf(typeof(ICollection<>)) ||
				type.IsGenericTypeOf(typeof(IReadOnlyCollection<>)) ||
				type.IsGenericTypeOf(typeof(List<>)) ||
				type.IsGenericTypeOf(typeof(IReadOnlyList<>)) ||
				type.IsGenericTypeOf(typeof(IList<>));
		}
		public virtual Models.Type GenerateAndAddType(Type type)
		{

			var re = new Models.Type
			{
				Name = FormatTypeName(type),
				IsEnumType = type.IsEnumType(),
				IsInterface = type.IsInterfaceType()
			};
			TypeCollection.AddType(re);
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
				if (IsGenericArrayType(type))
				{
					re.ElementType = ResolveType(type.GetGenericArguments()[0]);
					re.IsArrayType = true;
					return re;
				}
			}
			if (type.IsEnumType())
			{
				re.Properties = type
					.GetFields(BindingFlags.Public | BindingFlags.Static )
					.Select(p =>
					{
						var da = p.GetCustomAttribute<CommentAttribute>();
						var i= new Models.Property
						{
							Name = p.Name,
							Type = re.Name,
							Title=da?.Name,
							Description=da?.Description,
							Prompt=da?.Prompt,
							Group=da?.GroupName,
						};
                        LoadAttributes(i, p.GetCustomAttributes(), p);
                        return i;
                    })
					.ToArray();
				return re;
			}
			if (IsRootType(type))
				return re;
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
			LoadAttributes(re,type.GetCustomAttributes(true).Cast<Attribute>(), type);
			return re;
		}
		public virtual string FormatTypeName(Type type)
		{
			if (type.IsArray)
				return FormatTypeName(type.GetElementType()) + "[]";
			if (type.IsGeneric())
			{
				var dict_type = TryDetectDictType(type);
				if (dict_type != null)
					return FormatTypeName(dict_type) + "{}";


				if (IsGenericArrayType(type))
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
		//public IEnumerable<System.Reflection.MethodInfo> GetTypeMethods(Type type)
		//{
		//	if (!type.IsInterfaceType())
		//	{

		//	}
		//}

		public virtual IEnumerable< PropertyInfo> GetTypeProperties(Type type)
		{
			return type.GetProperties(
				BindingFlags.Public |
				BindingFlags.Instance |
				BindingFlags.FlattenHierarchy
				)
				.Where(p => p.DeclaringType == type && !typeof(Delegate).IsAssignableFrom( p.PropertyType));
		}
		public virtual Models.Property[] GenerateTypeProperties(Type type)
		{
            object obj = null;
            if (!type.IsAbstractType() && !type.IsInterfaceType() && !type.IsGenericDefinition())
            {
                var ci = type.GetConstructor(Array.Empty<Type>());
                if (ci != null)
                    obj = Activator.CreateInstance(type);
            }
			return GetTypeProperties(type)
				.Select(p => GenerateTypeProperty(p, obj))
				.ToArray();
		}

        static object TypeDefaultValue<T>(){
            return default(T);
        }
        static MethodInfo MethodGetTypeDefaultValue = typeof(BaseMetadataBuilder).GetMethod(
			"TypeDefaultValue",
            BindingFlags.Static | BindingFlags.NonPublic
            );
        static object GetTypeDefaultValue(Type type)
        {
            var m = MethodGetTypeDefaultValue.MakeGenericMethod(type);
            return m.Invoke(null,Array.Empty<object>());
        }
		public virtual Models.Property GenerateTypeProperty(PropertyInfo prop,object DefaultValueObject)
		{
			var attrs = prop.GetCustomAttributes(true).Cast<Attribute>();
			var prop_type = prop.PropertyType;
            bool optional;
			if (prop_type.IsGeneric() && 
				(prop_type.GetGenericTypeDefinition() == typeof(Nullable<>) ||
				prop_type.GetGenericTypeDefinition() == typeof(Option<>) ||
				prop_type.GetGenericTypeDefinition() == typeof(Lazy<>) ||
				prop_type.GetGenericTypeDefinition() == typeof(Func<>)
				)
				)
			{
				prop_type = prop_type.GetGenericArguments()[0];
                optional = true;
            }
            else
                optional = !prop_type.IsValue();

            if (prop_type == typeof(bool))
                optional = true;


            if (attrs.Any(a => a is OptionalAttribute /*|| a is ReadOnlyAttribute && ((ReadOnlyAttribute)a).IsReadOnly*/))
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
            }, attrs, prop);
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
			typeof(CommentAttribute),
			typeof(System.ComponentModel.DataAnnotations.RequiredAttribute),
            typeof(System.Runtime.InteropServices.OptionalAttribute),
			typeof(CategoryAttribute)
        };

		protected virtual Type[] GetIgnoreAttributeTypes()
		{
			return DefaultIgnoreAttributeTypes;
		}

		public virtual T LoadAttributes<T>(T item,IEnumerable<Attribute> attrs, object attrSource, Predicate<Attribute> predicate=null)where T : Models.Entity
		{
			var display = (CommentAttribute)
				attrs.FirstOrDefault(a => a is CommentAttribute);
			if (display != null)
			{
				item.Title = display.Name;
				item.Description = display.Description;
				item.Group = display.GroupName;
				item.ShortName = display.ShortName;
			}
			var cat = attrs.FirstOrDefault(a => a is CategoryAttribute) as CategoryAttribute;
			if (cat != null)
				item.Categories = cat.Names;

			var re = attrs
				.Where(a=>!GetIgnoreAttributeTypes().Contains(a.GetType()))
				.Where(a => (predicate==null || predicate(a)) && a.GetType().IsPublicType())
				.Select(a => GenerateAttribute(attrSource,a)).ToArray();
			if(re.Length>0)
				item.Attributes=re;
			return item;
		}
		protected virtual IMetadataAttributeValuesProvider TryGetAttributeValuesProvider(Attribute attr)
			=> null;

		protected virtual Models.Attribute GenerateAttribute(object attrSource,Attribute attr)
		{
			var type = attr.GetType();
			var valuesProvider = TryGetAttributeValuesProvider(attr);
			var values = JsonSerializer.Serialize(valuesProvider==null?attr:valuesProvider.GetValues(attr, attrSource));
			return new Models.Attribute
			{
				Type = type.FullName,
				Values = (values?.Length??0) > 0 && values!="{}" ? values : null
			};
		}
		
	}

	

}
