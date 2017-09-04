using SF.Core.DI;
using SF.Core.ServiceManagement.Internals;
using SF.Core.ServiceManagement.Storages;
using System.Linq;
using SF.Metadata;
using System;
using System.Reflection;
using SF.Core.Serialization;
using System.Collections.Generic;
using SF.Metadata.Models;
using System.ComponentModel.DataAnnotations;

namespace SF.Core.ServiceManagement
{
	class ImplementConfigTypeSource : NetworkService.IExtraServiceTypeSource
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
				return TryGenerateManagedServiceProperty(this,ServiceDetector, prop.PropertyType, prop.Name,prop.GetCustomAttributes(true)) ??
					TryGenerateDictionaryToArrayProperty(this,  prop.PropertyType, prop.Name, prop.GetCustomAttributes(true)) ??
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
								new SF.Metadata.EntityIdentAttribute(typeof(Management.IServiceInstanceManager)) {
									
									ScopeField =nameof(Models.ServiceInstanceInternal.ServiceType),
									ScopeValue=realType.GetFullName()
								}
						}),
					prop
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
				null
				);
		}
		SF.Metadata.Models.Property GenerateArgsProperty(System.Type Type, ParameterInfo Arg, IMetadataBuilder Builder)
		{
			return TryGenerateManagedServiceProperty(Builder, Metadata, Arg.ParameterType,Arg.Name,Arg.GetCustomAttributes(true)) ??
				TryGenerateDictionaryToArrayProperty(Builder, Arg.ParameterType, Arg.Name, Arg.GetCustomAttributes(true))??
				Builder.LoadAttributes(
					new Property
					{
						Name=Arg.Name,
						Type=Builder.TryGenerateAndAddType(Arg.ParameterType).Name
					},
					Arg.GetCustomAttributes(true).Cast<System.Attribute>(),
					null
					);
		}
		SF.Metadata.Models.Type GenerateArgsType(System.Type Type, ParameterInfo[] Args, IMetadataBuilder Builder)
		{
			var Title = (Type.GetCustomAttribute<CommentAttribute>()?.Name ?? Type.Name)+"����";
			var re = new SF.Metadata.Models.Type
			{
				Name = Type.GetFullName() + "SettingType",
				//Title = Title
			};
			re.Properties = Args.Where(
				arg => !arg.ParameterType.IsInterfaceType() || 
						Metadata.IsService(arg.ParameterType)
						)
				.Select(
				arg => GenerateArgsProperty(Type, arg, Builder)
				).ToArray();
			return re;
		}
		void GenerateImplConfig(System.Type Type, IMetadataBuilder Builder)
		{
			var ci = ServiceCreatorBuilder.FindBestConstructorInfo(Type);
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
				where t!=null// && IsArgTypeSupportted(t)
				select t
				).Distinct())
			{
				GenerateImplConfig(type, argTypeBuilder);
			}
		}
	}

}