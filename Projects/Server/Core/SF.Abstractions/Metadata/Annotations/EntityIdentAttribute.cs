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
using System.Text;
using System.Linq;
using SF.Entities;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = true)]
	public class EntityIdentAttribute : Attribute
	{
		public Type EntityType { get; }
		public string NameField { get; }
		public int Column { get; }
		public string ScopeField { get; set; }
		public object ScopeValue { get; set; }
		public bool IsTreeParentId { get; set; }
		public string MultipleKeyField { get; set; }

		public EntityIdentAttribute(Type EntityType = null, string NameField = null, int Column = 0, string MultipleKeyField = null)
		{
			//if(EntityManagementType != null)
			//{
			//	if (EntityManagementType.IsGenericTypeDefinition)
			//		throw new NotSupportedException();

			//	var loadable = GetAllInterfaces(EntityManagementType)
			//		.Where(t =>
			//			t.IsInterface &&
			//			t.IsGenericType &&
			//			t.GetGenericTypeDefinition() == typeof(IEntityLoadable<,>)
			//			)
			//		.FirstOrDefault();
			//	if (loadable == null)
			//		throw new NotSupportedException($"指定的类型{EntityManagementType}未实现IEntityLoadable<,>接口");


			//	this.EntityManagementType = EntityManagementType;
			//}

			this.EntityType = EntityType;
			this.NameField = NameField;
			this.Column = Column;
			this.MultipleKeyField = MultipleKeyField;
		}
	}
	
}
