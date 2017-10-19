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
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;
using System.Reflection.Emit;
using SF.Core.ServiceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Entities.AutoEntityProvider.Internals.EntityModifiers
{
	public class UpdatedTimePropertyModifierProvider : IEntityPropertyModifierProvider
	{
		public static int DefaultPriority { get; } = -10000;
		class UpdatedTimePropertyModifier : IEntityPropertyModifier<DateTime>
		{
			public int Priority => DefaultPriority;

			public DateTime Execute(IDataSetEntityManager Manager, IEntityModifyContext Context)
			{
				return Manager.Now;
			}
		}
		public IEntityPropertyModifier GetPropertyModifier(
			DataActionType ActionType, 
			Type EntityType, 
			PropertyInfo EntityProperty, 
			Type DataModelType, 
			PropertyInfo DataModelProperty
			)
		{   //������long����
			if (DataModelProperty.PropertyType != typeof(DateTime))
				return null;
			if (DataModelProperty.GetCustomAttribute<UpdatedTimeAttribute>() == null)
				return null;
			//�Զ��������������޸�
			if (ActionType != DataActionType.Create && ActionType!=DataActionType.Update)
				return new NonePropertyModifier(DefaultPriority);

			return new UpdatedTimePropertyModifier();
		}
	}
}