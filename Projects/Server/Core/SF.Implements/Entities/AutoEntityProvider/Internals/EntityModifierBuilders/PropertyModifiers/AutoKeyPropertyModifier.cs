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
	public class AutoKeyPropertyModifierProvider : IEntityPropertyModifierProvider
	{
		public static int DefaultPriority { get; } = -10000;
		class AutoKeyPropertyModifier : IAsyncEntityPropertyModifier<long>
		{
			public int Priority => DefaultPriority;

			public Task<long> Execute(IDataSetEntityManager Manager, IEntityModifyContext Context, long OrgValue)
			{
				return Manager.IdentGenerator.GenerateAsync(
					Manager.ServiceInstanceDescroptor.ServiceDeclaration.ServiceType.FullName
					);
			}


			public IEntityPropertyModifier Merge(IEntityPropertyModifier LowPriorityModifier)
			{
				return this;
			}
		}
		public IEntityPropertyModifier GetPropertyModifier(
			DataActionType ActionType, 
			Type EntityType, 
			PropertyInfo EntityProperty, 
			Type DataModelType, 
			PropertyInfo DataModelProperty
			)
		{   //必须是long类型
			if (DataModelProperty.PropertyType != typeof(long))
				return null;

			//不是主键字段
			if (!DataModelProperty.IsDefined(typeof(KeyAttribute)))
				return null;

		
			//使用数据库生成
			if ((DataModelProperty.GetCustomAttribute<DatabaseGeneratedAttribute>()?.DatabaseGeneratedOption ?? DatabaseGeneratedOption.Identity) != DatabaseGeneratedOption.None)
				return null;

			//使用数据库生成
			if (EntityProperty.GetCustomAttribute<DatabaseGeneratedAttribute>() != null)
				return null;

			//有多个主键字段
			if (DataModelType.AllPublicInstanceProperties().Any(p => p != DataModelProperty && p.IsDefined(typeof(KeyAttribute))))
				return null;

			//自动生成主键不能修改
			if (ActionType != DataActionType.Create)
				return new NonePropertyModifier(DefaultPriority);

			return new AutoKeyPropertyModifier();
		}
	}
}
