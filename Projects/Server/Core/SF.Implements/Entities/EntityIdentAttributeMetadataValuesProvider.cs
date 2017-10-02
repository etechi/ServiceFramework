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
		IEntityMetadataCollection EntityMetadataCollection { get; }
		public EntityIdentAttributeMetadataValuesProvider(
			IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver,
			IEntityMetadataCollection EntityMetadataCollection
			)
		{
			this.ServiceDeclarationTypeResolver = ServiceDeclarationTypeResolver;
			this.EntityMetadataCollection = EntityMetadataCollection;
		}
		public object GetValues(Attribute Attribute,object AttrSource)
		{
			var attr = (SF.Metadata.EntityIdentAttribute)Attribute;
			string entity = null;
			if (attr.EntityType != null)
			{
				var meta = EntityMetadataCollection.FindByEntityType(attr.EntityType);
				//var EntityManagementType = meta.EntityManagerType;
				//EntityManagerAttributeMetadataValuesProvider.VerifyEntityManagementType(EntityManagementType);
				entity = meta.Ident;//ServiceDeclarationTypeResolver.GetTypeIdent(EntityManagementType);
			}
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
