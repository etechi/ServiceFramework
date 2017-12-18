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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Plans.Manager.Models
{
	public class CallInstance 
	{
		/// <summary>
		/// 调用类型
		/// </summary>
		[Key]
		[MaxLength(200)]
		[Required]
		[Column(Order=1)]
		public string Type { get; set; }
		/// <summary>
		/// 调用标识
		/// </summary>
		[Key]
		[MaxLength(200)]
		[Required]
		[Column(Order = 2)]
		public string Ident { get; set; }

		/// <summary>
		/// 调用时间
		/// </summary>
		public DateTime CallTime { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[MaxLength(100)]
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateTime { get; set; }
		/// <summary>
		/// 过期时间
		/// </summary>
		public DateTime Expire { get; set; }

		/// <summary>
		/// 错误延时
		/// </summary>
		public int DelaySecondsOnError { get; set; }

		/// <summary>
		/// 最后执行时间
		/// </summary>
		public DateTime? LastExecTime { get; set; }
		/// <summary>
		/// 错误次数
		/// </summary>
		public int ErrorCount { get; set; }

	}
	
	public class CallInstanceEditable : CallInstance
	{

	}
}
