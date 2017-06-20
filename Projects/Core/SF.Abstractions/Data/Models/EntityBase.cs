using SF.Data.Storage;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SF.Data.Models
{

    public abstract class EntityBase<K> : IObjectWithId<K>
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
		public virtual LogicObjectState ObjectState { get; set; }


		[Comment("创建时间")]
		[TableVisible]
		public virtual DateTime CreatedTime { get; set; }

		[Comment("修改时间")]
		[TableVisible]
		public virtual DateTime UpdatedTime { get; set; }
	}

	public class EntityBase : EntityBase<long>
	{

	}
}
