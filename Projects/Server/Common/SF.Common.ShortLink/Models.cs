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

using SF.Sys.Entities.Models;
using SF.Sys.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.ShortLinks
{
    /// <summary>
    /// 短链接
    /// </summary>
	[EntityObject]
	public class ShortLink : ObjectEntityBase<string>
	{
		/// <summary>
		/// 目标
		/// </summary>
		[TableVisible]
		public string Target { get; set; }

		/// <summary>
		/// Post数据
		/// </summary>
		[MultipleLines]
		public string PostData { get; set; }

		/// <summary>
		/// 过期时间
		/// </summary>
		[TableVisible]
		public DateTime ExpireTime { get; set; }

	}
}
