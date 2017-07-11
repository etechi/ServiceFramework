using SF.Data.Storage;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SF.Data.DataModels
{

    public abstract class DataEntityBase<K> : IObjectWithId<K>
		where K:IEquatable<K>
	{

		[Comment("Id")]
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public virtual K Id { get; set; }

		[Comment("名称")]
		[MaxLength(100)]
		[Index]
		[Required]
		public virtual string Name { get; set; }

		
		[Comment("对象状态")]
		public virtual LogicObjectState ObjectState { get; set; }

		[Comment("所有人")]
		[Index]
		public virtual long OwnerId { get; set; }

		[Comment("创建时间")]
		[Index]
		public virtual DateTime CreatedTime { get; set; }

		
		[Comment("修改人")]
		[Index]
		public virtual long UpdatorId { get; set; }

		[Comment("修改时间")]
		public virtual DateTime UpdatedTime { get; set; }

	
		[ConcurrencyCheck]
		[Timestamp]
		[Comment(Name = "乐观锁时间戳")]
		public virtual byte[] TimeStamp { get; set; }

	}

	public class DataEntityBase : DataEntityBase<long>
	{

	}
}
