using SF.Data;
using SF.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace SF.Core.AppServices.Models
{
	[EntityObject("应用功能")]
	public class SysFeature :
		IObjectWithId<long>
	{
		[Key]
		[Comment("ID")]
		[ReadOnly(true)]
		[TableVisible]
		public long Id { get; set; }

		[Comment("名称")]
		[ReadOnly(true)]
		[TableVisible]
		public string Name { get; set; }

		[Comment("描述")]
		[ReadOnly(true)]
		[TableVisible]
		public string Description { get; set; }

	}
}

