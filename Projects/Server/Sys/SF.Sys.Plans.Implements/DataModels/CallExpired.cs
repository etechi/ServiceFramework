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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Sys.Plans.DataModels
{
	///<summary>已过期实例</summary>
	[Table("CallExpired")]
    public class CallExpired
	{
		
		/// <summary>
		/// 调用类型
		/// </summary>
		[MaxLength(200)]
		[Required]
		[Key]
		[Column(Order =1)]
		public string Type { get; set; }

		/// <summary>
		/// 调用标识
		/// </summary>
		[MaxLength(200)]
		[Required]
		[Key]
		[Column(Order = 2)]
		public string Ident { get; set; }

		/// <summary>
		/// 过期时间
		/// </summary>
		public DateTime Expired { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateTime { get; set; }

		/// <summary>
		/// 标题
		/// </summary>
		[MaxLength(100)]
        public string Title { get; set; }

		/// <summary>
		/// 调用参数
		/// </summary>
		[MaxLength(200)]
        public string CallArgument { get; set; }

		/// <summary>
		/// 调用异常
		/// </summary>
		[MaxLength(200)]
        public string CallError { get; set; }

		/// <summary>
		/// 最后执行时间
		/// </summary>
		public DateTime? LastExecTime { get; set; }

		/// <summary>
		/// 错误次数
		/// </summary>
		public int ExecCount { get; set; }
		/// <summary>
		/// 执行异常信息
		/// </summary>
		[MaxLength(200)]
		public string ExecError { get; set; }
	}
}
