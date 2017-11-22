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
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Sys.Metadata;
using SF.Entities;
using SF.Sys.Entities;
using SF.Sys.Annotations;

namespace SF.Sys.Services
{
	public class EntityObjectAttributeMetadataValuesProvider : IMetadataAttributeValuesProvider<EntityObjectAttribute>
	{
		IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver { get; }
		IEntityMetadataCollection EntityMetadataCollection { get; }
		public EntityObjectAttributeMetadataValuesProvider(
			IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver,
			IEntityMetadataCollection EntityMetadataCollection
			)
		{
			this.ServiceDeclarationTypeResolver = ServiceDeclarationTypeResolver;
			this.EntityMetadataCollection = EntityMetadataCollection ?? throw new ArgumentNullException(nameof(EntityMetadataCollection));
		}
		public object GetValues(Attribute Attribute,object AttrSource)
		{
			//var attr = (SF.Metadata.EntityObjectAttribute)Attribute;
			var meta = EntityMetadataCollection.FindByEntityType((Type)AttrSource);
			//var EntityManagementType = meta.EntityManagerType;
			//EntityManagerAttributeMetadataValuesProvider.VerifyEntityManagementType(EntityManagementType);
			var entity = meta?.Ident;//ServiceDeclarationTypeResolver.GetTypeIdent(EntityManagementType);
			if (entity.IsNullOrEmpty())
				return null;
			return new
			{
				Entity = entity,
			};

		}
	}
}
