using SF.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Data.IdentGenerator.DataModels
{
	[Table("SysIdentSeed")]
	public class IdentSeed
	{
		[Key]
		[Comment(Name ="类型")]
		public string Type { get; set; }

		[Comment(Name = "下一个主键标识值")]
		public long NextValue { get; set; }

		[Comment(Name = "乐观锁时间戳")]
		[ConcurrencyCheck]
		[Timestamp]
		public byte[] TimeStamp { get; set; }
	}
}
