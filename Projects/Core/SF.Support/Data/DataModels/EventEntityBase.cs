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

    public abstract class EventEntityBase<K> : IObjectWithId<K>, IEventEntity
		where K:IEquatable<K>
	{
		[Comment("Id")]
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public virtual K Id { get; set; }

		[Comment("区域")]
		[Index]
		public virtual long? ScopeId { get; set; }

		[Comment("用户")]
		[Index]
		public virtual long? UserId { get; set; }

		[Comment("时间")]
		[Index]
		public virtual DateTime Time { get; set; }

	}

	public class EventEntityBase : EventEntityBase<long>
	{

	}
}
