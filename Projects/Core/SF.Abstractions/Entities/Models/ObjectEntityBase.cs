using SF.Entities;
using SF.Metadata;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace SF.Data.Models
{


	public abstract class ObjectEntityBase<K> : IEntityWithId<K>, IObjectEntity
		where K:IEquatable<K>
	{
		[Comment("Id")]
		[Key]
		[ReadOnly(true)]
		[TableVisible]
		public virtual K Id { get; set; }

		[Comment("名称")]
		[MaxLength(100)]
		[Index]
		[EntityTitle]
		[TableVisible]
		[Required]
		public virtual string Name { get; set; }
		
		[Comment("对象状态")]
		[TableVisible]
		public virtual LogicEntityState ObjectState { get; set; }


		[Comment("创建时间")]
		[TableVisible]
		[ReadOnly(true)]
		public virtual DateTime CreatedTime { get; set; }

		[Comment("修改时间")]
		[TableVisible]
		[ReadOnly(true)]
		public virtual DateTime UpdatedTime { get; set; }
	}

	public class ObjectEntityBase : ObjectEntityBase<long>
	{

	}
}
