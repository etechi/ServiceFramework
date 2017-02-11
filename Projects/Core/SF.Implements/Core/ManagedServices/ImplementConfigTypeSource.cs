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
				if(prop.PropertyType.IsInterfaceType() && ServiceDetector.GetServiceType(prop.PropertyType)==ServiceType.Managed)
				{
					return LoadAttributes(
						new Property
						{
							Name=prop.Name,
							Type="string"
						},
						prop.GetCustomAttributes(true).Cast<System.Attribute>().Union(
							new System.Attribute[]
							{
								new RequiredAttribute(),
								new SF.Metadata.EntityIdentAttribute("系统服务实例") {
									ScopeField =nameof(Models.ServiceInstance.DeclarationId),
									ScopeValue=prop.PropertyType.FullName
								}
							})
						);
				}
				return base.GenerateTypeProperty(prop, DefaultValueObject);
			}
		}

		IServiceMetadata Metadata { get; }
		public ImplementConfigTypeSource(IServiceMetadata Metadata)
		{
			this.Metadata = Metadata;
		}
		SF.Metadata.Models.Property GenerateArgsProperty(System.Type Type, ParameterInfo Arg, IMetadataBuilder Builder)
		{
			if (Arg.ParameterType.IsInterfaceType() && Metadata.GetServiceType(Arg.ParameterType) == ServiceType.Managed)
			{
				return Builder.LoadAttributes(
					new Property
					{
						Name = Arg.Name,
						Type = "string"
					},
					Arg.GetCustomAttributes(true).Cast<System.Attribute>().Union(
						new System.Attribute[]
						{
							new RequiredAttribute(),
							new SF.Metadata.EntityIdentAttribute("系统服务实例") {
								ScopeField =nameof(Models.ServiceInstance.DeclarationId),
								ScopeValue=Arg.ParameterType.FullName
							}
						})
				);
			}

			return Builder.LoadAttributes(
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
