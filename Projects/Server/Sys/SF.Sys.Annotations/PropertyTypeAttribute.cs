﻿#region Apache License Version 2.0
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

namespace SF.Sys.Annotations
{

	public enum PropertyTypeSourceType
	{
		Internal,
		External
	}

	/// <summary>
	/// 指定属性实体类型字段
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class PropertyTypeAttribute : Attribute
	{
		public PropertyTypeSourceType TypeSourceType { get; }
		public string TypeSourceField { get; }
		public PropertyTypeAttribute(PropertyTypeSourceType TypeSourceType, string TypeSourceField)
		{
			this.TypeSourceType = TypeSourceType;
			this.TypeSourceField = TypeSourceField;
		}
	}

}
