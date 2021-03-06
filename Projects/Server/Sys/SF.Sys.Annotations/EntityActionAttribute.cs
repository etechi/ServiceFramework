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
	public enum ActionMode
	{
		/// <summary>
		 /// 浏览模式
		 /// </summary>
		View,
		/// <summary>
		/// 编辑模式
		/// </summary>
		Edit,
		/// <summary>
		/// 载入模式
		/// </summary>
		Post,
		/// <summary>
		/// 更新模式
		/// </summary>
		Update,
		/// <summary>
		/// 查询模式
		/// </summary>
		Query
	}
	/// <summary>
	/// 指定实体方法
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class EntityActionAttribute : Attribute
	{
		public ActionMode Mode { get; set; }
		public string ConditionExpression { get; set; }
	}

}
