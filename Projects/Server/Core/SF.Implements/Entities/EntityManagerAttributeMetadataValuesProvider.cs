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
using SF.Metadata;
using SF.Entities;

namespace SF.Core.ServiceManagement
{
	public class EntityManagerAttributeMetadataValuesProvider : IMetadataAttributeValuesProvider<SF.Metadata.EntityManagerAttribute>
	{
		IEntityMetadataCollection EntityMetadataCollection { get; }
		public EntityManagerAttributeMetadataValuesProvider(IEntityMetadataCollection EntityMetadataCollection)
		{
			this.EntityMetadataCollection = EntityMetadataCollection;
		}
		//public static void VerifyEntityManagementType(Type EntityManagementType)
		//{
		//	if (EntityManagementType.IsGenericTypeDefinition)
		//		throw new NotSupportedException($"实体管理服务不能是泛型定义:{EntityManagementType}");
		//	if (!EntityManagementType.IsInterfaceType())
		//		throw new NotSupportedException($"实体管理服务必须是接口:{EntityManagementType}");

		//	if (!EntityManagementType.IsDefined(typeof(EntityManagerAttribute)))
		//		throw new NotSupportedException($"实体管理服务{EntityManagementType}未定义属性EntityManagerAttribute");

		//	var loadable = EntityManagementType.AllInterfaces()
		//		.Where(t =>
		//			t.IsInterface &&
		//			t.IsGenericType &&
		//			t.GetGenericTypeDefinition() == typeof(IAbstractEntityManager<>)
		//			)
		//		.FirstOrDefault();
		//	if (loadable == null)
		//		throw new NotSupportedException($"指定的类型{EntityManagementType}未实现IAbstractEntityManager<>接口");

		//}
		public object GetValues(Attribute Attribute,object AttrSource)
		{	
			var attr = (SF.Metadata.EntityManagerAttribute)Attribute;
			var EntityManagementType = (Type)AttrSource;
			var entity = EntityMetadataCollection.FindByManagerType(EntityManagementType);
			//VerifyEntityManagementType(EntityManagementType);
			//var entity = ServiceDeclarationTypeResolver.GetTypeIdent(EntityManagementType);
			return new
			{
				Entity = entity.Ident,
				Title= entity.Name,
				//FontIcon = entity.FontIcon
			};

		}
	}
}
