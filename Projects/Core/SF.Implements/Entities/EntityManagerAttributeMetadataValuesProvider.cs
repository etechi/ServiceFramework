using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Metadata;
using SF.Entities;

namespace SF.Core.ServiceManagement
{
	public class EntityManagerAttributeMetadataValuesProvider : IMetadataAttributeValuesProvider<SF.Metadata.EntityManagerAttribute>
	{
		IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver { get; }
		public EntityManagerAttributeMetadataValuesProvider(IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver)
		{
			this.ServiceDeclarationTypeResolver = ServiceDeclarationTypeResolver;
		}
		public static void VerifyEntityManagementType(Type EntityManagementType)
		{
			if (EntityManagementType.IsGenericTypeDefinition)
				throw new NotSupportedException($"实体管理服务不能是泛型定义:{EntityManagementType}");
			if (!EntityManagementType.IsInterfaceType())
				throw new NotSupportedException($"实体管理服务必须是接口:{EntityManagementType}");

			if (!EntityManagementType.IsDefined(typeof(EntityManagerAttribute)))
				throw new NotSupportedException($"实体管理服务{EntityManagementType}未定义属性EntityManagerAttribute");

			var loadable = EntityManagementType.AllInterfaces()
				.Where(t =>
					t.IsInterface &&
					t.IsGenericType &&
					t.GetGenericTypeDefinition() == typeof(IAbstractEntityManager<>)
					)
				.FirstOrDefault();
			if (loadable == null)
				throw new NotSupportedException($"指定的类型{EntityManagementType}未实现IAbstractEntityManager<>接口");

		}
		public object GetValues(Attribute Attribute,object AttrSource)
		{
			var attr = (SF.Metadata.EntityManagerAttribute)Attribute;
			var EntityManagementType = (Type)AttrSource;
			VerifyEntityManagementType(EntityManagementType);
			var entity = ServiceDeclarationTypeResolver.GetTypeIdent(EntityManagementType);
			return new
			{
				Entity = entity,
				Title=attr.Title,
				FontIcon = attr.FontIcon
			};

		}
	}
}
