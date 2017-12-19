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
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Data;
using SF.Sys.Plans.Manager;

namespace SF.Sys.Plans.DataModels
{
	/// <summary>
	/// 调用实例
	/// </summary>
	[Table("CallInstance")]
    public class CallInstance : ICallInstance
	{
		/// <summary>
		/// 调用类型
		/// </summary>
		[Key]
		[MaxLength(200)]
		[Required]
		[Column(Order =1)]
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
		/// 服务范围ID
		/// </summary>
		public long? ServiceScopeId { get; set; }

		/// <summary>
		/// 调用时间
		/// </summary>
		[Index]
        public DateTime CallTime { get; set; }
		/// <summary>
		/// 过期时间
		/// </summary>
		public DateTime Expire { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateTime { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[MaxLength(100)]
		[Required]
        public string Name { get; set; }

		/// <summary>
		/// 错误延时
		/// </summary>
		public int DelaySecondsOnError { get; set; }

		/// <summary>
		/// 调用参数
		/// </summary>
		[MaxLength(200)]
        public string Argument { get; set; }

		/// <summary>
		/// 错误信息
		/// </summary>
		[MaxLength(200)]
        public string Error { get; set; }

		/// <summary>
		/// 最后执行时间
		/// </summary>
		public DateTime? LastExecTime { get; set; }
		/// <summary>
		/// 错误次数
		/// </summary>
		public int ErrorCount { get; set; }

		/// <summary>
		/// 执行异常信息
		/// </summary>
		[MaxLength(200)]
        public string ExecError { get; set; }

		/// <summary>
		/// 乐观锁时间戳
		/// </summary>
		[ConcurrencyCheck]
		[Timestamp]
        public byte[] TimeStamp { get; set; }
	}
}
