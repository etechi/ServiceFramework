using SF.Core.DI;
using SF.Core.ManagedServices.Runtime;
using SF.Core.ManagedServices.Storages;
using System.Linq;
using SF.Metadata;
using System;
using System.Reflection;
using SF.Core.Serialization;
using System.Collections.Generic;
using SF.Metadata.Models;
using System.ComponentModel.DataAnnotations;

namespace SF.Core.ManagedServices
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
					ServiceDetector.GetServiceType(prop.PropertyType) == ServiceType.Managed
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
			if (realType.IsInterfaceType() && ServiceDetector.GetServiceType(realType) == ServiceType.Managed)
			{
				return Builder.LoadAttributes(
					new Property
					{
						Name = Name,
						Type = "string"
					},
					Attributes.Cast<System.Attribute>().Union(
						new System.Attribute[]
						{
								new RequiredAttribute(),
								new SF.Metadata.EntityIdentAttribute("系统服务实例") {
									ScopeField =nameof(Models.ServiceInstanceInternal.DeclarationId),
									ScopeValue=realType.FullName
								}
						})
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
				Attributes.Cast<System.Attribute>()
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
					Arg.GetCustomAttributes(true).Cast<System.Attribute>()
					);
		}
		SF.Metadata.Models.Type GenerateArgsType(System.Type Type, ParameterInfo[] Args, IMetadataBuilder Builder)
		{
			var Title = (Type.GetCustomAttribute<CommentAttribute>()?.Name ?? Type.Name)+"设置";
			var re = new SF.Metadata.Models.Type
			{
				Name = Type.FullName + "CreateArguments",
				//Title = Title
			};
			re.Properties = Args.Where(
				arg => !arg.ParameterType.IsInterfaceType() || 
						Metadata.GetServiceType(arg.ParameterType) == ServiceType.Managed
						)
				.Select(
				arg => GenerateArgsProperty(Type, arg, Builder)
				).ToArray();
			return re;
		}
		void GenerateImplConfig(System.Type Type, IMetadataBuilder Builder)
		{
			var ci = Runtime.ServiceFactoryBuilder.FindBestConstructorInfo(Type);
			var args = ci.GetParameters();
			var type = GenerateArgsType(Type, args, Builder);
			Builder.TypeCollection.AddType(type);

		}
		public void AddExtraServiceType(IMetadataBuilder Builder)
		{
			var argTypeBuilder = new CreateArgumentMetadataBuilder(
				Metadata,
				Builder.JsonSerializer, 
				Builder.TypeCollection
				);
			foreach(var type in (from pair in Metadata.ManagedServices
					 from impl in pair.Value
					 select impl.Type).Distinct())
			{
				GenerateImplConfig(type, argTypeBuilder);
			}
		}
	}

}
