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

using SF.Sys.Services.Internals;
using System.Linq;
using System;
using System.Reflection;
using System.Collections.Generic;
using SF.Sys.NetworkService;
using SF.Sys.Metadata;
using SF.Sys.Serialization;
using SF.Sys.Reflection;
using SF.Sys.Metadata.Models;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Comments;
using SF.Sys.Annotations;
using SF.Sys.Services.Management.Models;

namespace SF.Sys.Services
{
	class ImplementConfigTypeSource : IExtraServiceTypeSource
	{
		class CreateArgumentMetadataBuilder : BaseMetadataBuilder
		{
			IServiceDetector ServiceDetector { get; }

			public CreateArgumentMetadataBuilder(
				IServiceDetector ServiceDetector,
				IJsonSerializer JsonSerializer,
				IMetadataTypeCollection Types
				) : 
				base(JsonSerializer, Types)
			{
				this.ServiceDetector = ServiceDetector;
			}
			
			public override IEnumerable<PropertyInfo> GetTypeProperties(System.Type type)
			{
				return base.GetTypeProperties(type).Where(prop =>
					!prop.PropertyType.IsInterfaceType() ||
					!(ServiceDetector.IsService(prop.PropertyType) ||
					prop.PropertyType.IsGeneric() && ServiceDetector.IsService(prop.PropertyType.GetGenericTypeDefinition())
					)
					);

			}
			
			public override Property GenerateTypeProperty(PropertyInfo prop, object DefaultValueObject)
			{
				return TryGenerateManagedServiceProperty(this,ServiceDetector, prop.PropertyType, prop.Name,prop.GetCustomAttributes()) ??
					TryGenerateDictionaryToArrayProperty(this,  prop.PropertyType, prop.Name, prop.GetCustomAttributes()) ??
					base.GenerateTypeProperty(prop, DefaultValueObject);
			}
		}

		IServiceMetadata Metadata { get; }
		public ImplementConfigTypeSource(IServiceMetadata Metadata)
		{
			this.Metadata = Metadata;
		}
		static Property TryGenerateManagedServiceProperty(
			IMetadataBuilder Builder,
			IServiceDetector ServiceDetector, 
			System.Type Type,
			string Name,
			IEnumerable<object> Attributes
			)
		{
			var realType = Type.GetGenericArgumentTypeAsLazy()
				?? Type.GetGenericArgumentTypeAsFunc()
				?? Type;
			if (realType.IsInterfaceType() && ServiceDetector.IsService(realType))
			{
				var prop = new Property
				{
					Name = Name,
					Type = "long",
					Optional=true
				};
				return Builder.LoadAttributes(
					prop,
					Attributes.Cast<System.Attribute>().Union(
						new System.Attribute[]
						{
								new EntityIdentAttribute(typeof(ServiceInstance)) {
									ScopeField =nameof(ServiceInstanceInternal.ServiceType),
									ScopeValue=realType.GetFullName()
								}
						}),
					prop,
					null
					);
			}
			return null;
		}
		static Property TryGenerateDictionaryToArrayProperty(
		   IMetadataBuilder Builder,
		   System.Type Type,
		   string Name,
		   IEnumerable<object> Attributes
		   )
		{
			if (!Type.IsGeneric())
				return null;
			if (Type.GetGenericTypeDefinition() != typeof(Dictionary<,>))
				return null;
			var gtypes = Type.GetGenericArguments();
			var keyProps = gtypes[1].GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Where(p => p.IsDefined(typeof(KeyAttribute))).ToArray();
			if (keyProps.Length != 1)
				return null;
			if (keyProps[0].PropertyType != gtypes[0])
				return null;
			return Builder.LoadAttributes(
				new Property
				{
					Name = Name,
					Type = Builder.GenerateAndAddType(gtypes[1].MakeArrayType()).Name
				},
				Attributes.Cast<System.Attribute>(),
				null,
				null
				);
		}
		SF.Sys.Metadata.Models.Property GenerateArgsProperty(System.Type Type, ParameterInfo Arg, IMetadataBuilder Builder)
		{
			return TryGenerateManagedServiceProperty(Builder, Metadata, Arg.ParameterType,Arg.Name,Arg.GetCustomAttributes()) ??
				TryGenerateDictionaryToArrayProperty(Builder, Arg.ParameterType, Arg.Name, Arg.GetCustomAttributes())??
				Builder.LoadAttributes(
					new Property
					{
						Name=Arg.Name,
						Type=Builder.TryGenerateAndAddType(Arg.ParameterType).Name
					},
					Arg.GetCustomAttributes(),
					null,
					null
					);
		}
		SF.Sys.Metadata.Models.Type GenerateArgsType(System.Type Type, ParameterInfo[] Args, IMetadataBuilder Builder)
		{
			var Title = (Type.Comment()?.Title ?? Type.Name)+"ÉèÖÃ";
			var re = new SF.Sys.Metadata.Models.Type
			{
				Name = Type.GetFullName() + "SettingType",
				//Title = Title
			};
			re.Properties = Args.Where(
				arg => (!arg.ParameterType.IsInterfaceType() || 
						Metadata.IsService(arg.ParameterType) ) &&
						!typeof(Delegate).IsAssignableFrom(arg.ParameterType)
						)
				.Select(
				arg => GenerateArgsProperty(Type, arg, Builder)
				).ToArray();
			return re;
		}
		void GenerateImplConfig(System.Type Type, IMetadataBuilder Builder)
		{
			var ci = ServiceCreatorBuilder.FindBestConstructorInfo(Type,Metadata);
			var args = ci.GetParameters();
			var type = GenerateArgsType(Type, args, Builder);
			Builder.TypeCollection.AddType(type);

		}
		static bool? IsArgTypeSupporttedInternal(System.Type type,Dictionary<System.Type,bool?> types)
		{
			if (types.TryGetValue(type, out var supported))
				return supported;
			types[type] = null;

			switch (type.GetTypeCode())
			{
				case TypeCode.Boolean:
				case TypeCode.Byte:
				case TypeCode.Char:
				case TypeCode.DateTime:
				case TypeCode.DBNull:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.String:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					supported=true;
					break;
				case TypeCode.Object:
					supported = type.GetProperties(
						BindingFlags.Public |
						BindingFlags.Instance |
						BindingFlags.GetProperty |
						BindingFlags.SetProperty |
						BindingFlags.FlattenHierarchy
						)
					.Aggregate(true,(s,p) =>s && (IsArgTypeSupporttedInternal(p.PropertyType, types)??s));
					break;
				default:
					supported= false;
					break;
			}
			types[type] = supported;
			return supported;
		}
		static bool IsArgTypeSupportted(System.Type type)
		{
			return IsArgTypeSupporttedInternal(type, new Dictionary<System.Type, bool?>()) ?? true;
			
		}
		public void AddExtraServiceType(IMetadataBuilder Builder)
		{
			var argTypeBuilder = new CreateArgumentMetadataBuilder(
				Metadata,
				Builder.JsonSerializer, 
				Builder.TypeCollection
				);
			foreach(var type in (
				from svc in Metadata.Services
				//where !svc.Value.ServiceType.IsDefined(typeof(UnmanagedServiceAttribute))
				from impl in svc.Value.Implements
				where impl.IsManagedService
				let t= impl.ImplementType
				where t!=null && !t.IsGenericTypeDefinition// && IsArgTypeSupportted(t)
				select t
				).Distinct())
			{
				GenerateImplConfig(type, argTypeBuilder);
			}
		}
	}

}
