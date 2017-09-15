using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Data.IdentGenerator.EFCore.DataModels
{
	[Table("SysIdentSeed")]
	public class IdentSeed
	{
		[Key]
		[Display(Name ="类型")]
		public string Type { get; set; }

		[Display(Name = "下一个主键标识值")]
		public long NextValue { get; set; }

		[Display(Name = "乐观锁时间戳")]
		[ConcurrencyCheck]
		[Timestamp]
		public byte[] TimeStamp { get; set; }
	}
}
