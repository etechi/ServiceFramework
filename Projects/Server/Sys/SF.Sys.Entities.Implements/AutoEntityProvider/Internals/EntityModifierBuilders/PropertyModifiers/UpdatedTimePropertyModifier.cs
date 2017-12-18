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
using System;
using System.Reflection;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.EntityModifiers
{
	public class UpdatedTimePropertyModifierProvider : IEntityPropertyModifierProvider
	{
		public static int DefaultPriority { get; } = -20000;
		class UpdatedTimePropertyModifier : IEntityPropertyModifier<DateTime>
		{
			public int MergePriority => DefaultPriority;
			public int ExecutePriority => 0;

			public DateTime Execute(IEntityServiceContext ServiceContext, IEntityModifyContext Context,DateTime OrgValue)
			{
				return ServiceContext.Now;
			}
			public IEntityPropertyModifier Merge(IEntityPropertyModifier LowPriorityModifier) => this;
		}
		public IEntityPropertyModifier GetPropertyModifier(
			DataActionType ActionType, 
			Type EntityType, 
			PropertyInfo EntityProperty, 
			Type DataModelType, 
			PropertyInfo DataModelProperty
			)
		{   //必须是long类型
			if (DataModelProperty.PropertyType != typeof(DateTime))
				return null;
			if (DataModelProperty.GetCustomAttribute<UpdatedTimeAttribute>() == null &&
				EntityProperty?.GetCustomAttribute<UpdatedTimeAttribute>() == null)
				return null;
			//自动生成主键不能修改
			if (ActionType != DataActionType.Create && ActionType!=DataActionType.Update)
				return new NonePropertyModifier(DefaultPriority);

			return new UpdatedTimePropertyModifier();
		}
	}
}
