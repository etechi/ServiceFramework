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

namespace SF.Sys.Annotations
{
	/// <summary>
	/// 用于标注当前字段为实体类型名称
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
	public class EntityTypeAttribute : Attribute
	{
        public string Tag { get; set; }

	}
    
    /// <summary>
    /// 用于标注当前字段为实体标识
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = true)]
	public class EntityIdentAttribute : Attribute
	{
		/// <summary>
		/// 实体类型,若指定，则当前字段为次实体类型的实体标识
		/// </summary>
		public Type EntityType { get; }

		/// <summary>
		/// 名称字段，实体名称字段
		/// </summary>
		public string NameField { get; }

		/// <summary>
		/// 列索引
		/// </summary>
		public int Column { get; }

		/// <summary>
		/// 筛选范围字段，若指定，则在此字段范围中选择实体
		/// </summary>
		public string ScopeField { get; set; }

		/// <summary>
		/// 筛选范围值，若制定，则在值范围中选择实体
		/// </summary>
		public object ScopeValue { get; set; }

		/// <summary>
		/// 是否为树结构父节点
		/// </summary>
		public bool IsTreeParentId { get; set; }

		/// <summary>
		/// 多主键字段
		/// </summary>
		public string MultipleKeyField { get; set; }

		/// <summary>
		/// 实体类型字段，若制定，则为此字段指定实体的实体标识
		/// </summary>
		public string EntityTypeField { get; set; }

        /// <summary>
        /// 是否包含业务类型
        /// </summary>
        public bool WithBizType { get; set; }

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
