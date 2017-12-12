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

using SF.Sys.Annotations;
using SF.Sys.Entities;
using SF.Sys.Metadata;
using System;

namespace SF.Sys.Services
{
	public class EntityIdentAttributeMetadataValuesProvider : IMetadataAttributeValuesProvider<EntityIdentAttribute>
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
			var attr = (EntityIdentAttribute)Attribute;
			string entity = null;
			if (attr.EntityType != null)
			{
				var meta = EntityMetadataCollection.FindByEntityType(attr.EntityType);
				if (meta == null)
					throw new InvalidOperationException($"实体库中找不类型为{attr.EntityType}实体");
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
