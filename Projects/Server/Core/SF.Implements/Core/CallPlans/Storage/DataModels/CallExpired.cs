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
using SF.Metadata;

namespace SF.Core.CallPlans.Storage.DataModels
{
	[Table("SysCallExpired")]
    [Comment(GroupName = "可靠调用服务", Name = "已过期实例")]
    public class CallExpired
	{
		[Key]
		[Required]
		[MaxLength(200)]
        [Comment(Name ="调用过程")]
		public string Callable { get; set; }

        [Comment(Name = "过期时间")]
        public DateTime Expired { get; set; }
        [Comment(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

		[MaxLength(100)]
        [Comment(Name = "标题")]

        public string Title { get; set; }

		[MaxLength(200)]
        [Comment(Name = "调用参数")]
        public string CallArgument { get; set; }

		[MaxLength(200)]
        [Comment(Name = "调用异常")]
        public string CallError { get; set; }

        [Comment(Name = "最后执行时间")]
        public DateTime? LastExecTime { get; set; }

        [Comment(Name = "错误次数")]
        public int ExecCount { get; set; }
        [Comment(Name = "执行异常信息")]
        [MaxLength(200)]
		public string ExecError { get; set; }
	}
}
