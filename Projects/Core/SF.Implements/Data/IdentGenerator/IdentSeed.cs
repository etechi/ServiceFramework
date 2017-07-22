using SF.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Data.IdentGenerator.DataModels
{
	[Table("SysIdentSeed")]
	public class IdentSeed
	{
		[Key]
		[Comment(Name ="范围")]
		[Column(Order =1)]
		public long ScopeId { get; set; }

		[Key]
		[Column(Order = 2)]
		[Comment(Name = "类型")]
		[MaxLength(100)]
		public string Type { get; set; }


		[Comment(Name = "下一个标识值")]
		public long NextValue { get; set; }

		[Comment(Name = "标识值分段",Description ="标识分段变化时，将重新开始生成标识值")]
		public int Section { get; set; }

		[Comment(Name = "乐观锁时间戳")]
		[ConcurrencyCheck]
		[Timestamp]
		public byte[] TimeStamp { get; set; }
	}
}
