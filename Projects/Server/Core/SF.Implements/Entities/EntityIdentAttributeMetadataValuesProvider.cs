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
	public class EntityIdentAttributeMetadataValuesProvider : IMetadataAttributeValuesProvider<SF.Metadata.EntityIdentAttribute>
	{
		IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver { get; }
		public EntityIdentAttributeMetadataValuesProvider(IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver)
		{
			this.ServiceDeclarationTypeResolver = ServiceDeclarationTypeResolver;
		}
		public object GetValues(Attribute Attribute,object AttrSource)
		{
			var attr = (SF.Metadata.EntityIdentAttribute)Attribute;
			var EntityManagementType = attr.EntityType;
			EntityManagerAttributeMetadataValuesProvider.VerifyEntityManagementType(EntityManagementType);
			var entity = ServiceDeclarationTypeResolver.GetTypeIdent(EntityManagementType);
			return new
			{
				Entity = entity,
				NameField = attr.NameField,
				Column = attr.Column,
				ScopeField = attr.ScopeField,
				ScopeValue = attr.ScopeValue,
				IsTreeParentId = attr.IsTreeParentId,
				MultipleKeyField = attr.MultipleKeyField
			};

		}
	}
}
