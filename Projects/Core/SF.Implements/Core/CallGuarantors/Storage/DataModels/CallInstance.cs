using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Data.Storage;

namespace SF.Core.CallGuarantors.Storage.DataModels
{
	[Table("SysCallInstance")]
    [Comment(GroupName = "可靠调用服务", Name = "调用实例")]
    public class CallInstance : ICallInstance
	{
		[Key]
		[MaxLength(200)]
		[Required]
		[Column(Order =1)]
        [Comment(Name ="调用过程名")]
		public string Callable { get; set; }

		[Index]
        [Comment(Name = "调用时间")]
        public DateTime CallTime { get; set; }
        [Comment(Name = "过期时间")]
        public DateTime Expire { get; set; }
        [Comment(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

		[MaxLength(100)]
		[Required]
        [Comment(Name = "标题")]
        public string Title { get; set; }

        [Comment(Name = "错误延时")]
        public int DelaySecondsOnError { get; set; }


		[MaxLength(200)]
        [Comment(Name = "调用参数")]
        public string CallArgument { get; set; }

		[MaxLength(200)]
        [Comment(Name = "错误信息")]
        public string CallError { get; set; }

        [Comment(Name = "最后执行时间")]
        public DateTime? LastExecTime { get; set; }
        [Comment(Name = "错误次数")]
        public int ErrorCount { get; set; }

        [MaxLength(200)]
        [Comment(Name = "执行异常信息")]
        public string ExecError { get; set; }

		[ConcurrencyCheck]
		[Timestamp]
        [Comment(Name = "乐观锁时间戳")]
        public byte[] TimeStamp { get; set; }
	}
}
