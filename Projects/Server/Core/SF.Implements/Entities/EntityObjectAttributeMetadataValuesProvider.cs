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
	public class EntityObjectAttributeMetadataValuesProvider : IMetadataAttributeValuesProvider<SF.Metadata.EntityObjectAttribute>
	{
		IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver { get; }
		IEntityMetadataCollection EntityMetadataCollection { get; }
		public EntityObjectAttributeMetadataValuesProvider(
			IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver,
			IEntityMetadataCollection EntityMetadataCollection
			)
		{
			this.ServiceDeclarationTypeResolver = ServiceDeclarationTypeResolver;
			this.EntityMetadataCollection = EntityMetadataCollection;
		}
		public object GetValues(Attribute Attribute,object AttrSource)
		{
			//var attr = (SF.Metadata.EntityObjectAttribute)Attribute;
			var meta = EntityMetadataCollection.FindByEntityType((Type)AttrSource);
			//var EntityManagementType = meta.EntityManagerType;
			//EntityManagerAttributeMetadataValuesProvider.VerifyEntityManagementType(EntityManagementType);
			var entity = meta.Ident;//ServiceDeclarationTypeResolver.GetTypeIdent(EntityManagementType);
			return new
			{
				Entity = entity,
			};

		}
	}
}
