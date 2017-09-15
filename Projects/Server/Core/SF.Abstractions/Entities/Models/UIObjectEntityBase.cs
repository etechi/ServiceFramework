using SF.Entities;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
namespace SF.Data.Models
{
	public class InternalData
	{
		[Comment("备注", "内部使用，不用于UI显示")]
		[MaxLength(200)]
		[MultipleLines]
		public virtual string Memo { get; set; }

	}
	public abstract class UIObjectEntityBase<K> : ObjectEntityBase<K>,IUIObjectEntity
		where K:IEquatable<K>
	{
		
		[Comment("标题", "用于前端显示",GroupName ="前端展示")]
		[MaxLength(100)]
		[Required]
		[TableVisible]
		public virtual string Title { get; set; }

		[Comment("副标题", "用于前端显示", GroupName = "前端展示")]
		[MaxLength(100)]
		public virtual string SubTitle { get; set; }

		[Comment("提示", "用于前端显示", GroupName = "前端展示")]
		[MaxLength(100)]
		public virtual string Remarks { get; set; }

		[Comment("说明", "用于前端显示", GroupName = "前端展示")]
		[MaxLength(2000)]
		[MultipleLines]
		public virtual string Description { get; set; }


		[Comment("图片", "大尺寸图片", GroupName = "前端展示")]
		[Image]
		public virtual string Image { get; set; }

		[Comment("图标", "小尺寸图片", GroupName = "前端展示")]
		[Image]
		public virtual string Icon { get; set; }

	}
	public abstract class UIObjectEntityBase : UIObjectEntityBase<long>
	{ }
	
}
