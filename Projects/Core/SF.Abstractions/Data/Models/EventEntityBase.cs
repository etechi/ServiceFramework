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


    public abstract class EventEntityBase<K> : IObjectWithId<K>, IEventEntity
		where K:IEquatable<K>
	{
		[Comment("Id")]
		[Key]
		[ReadOnly(true)]
		[TableVisible]
		public virtual K Id { get; set; }

	
		[Comment("时间")]
		[TableVisible]
		[ReadOnly(true)]
		public virtual DateTime Time{ get; set; }

		[Comment("用户")]
		[ReadOnly(true)]
		[EntityIdent("身份标识", nameof(UserName))]
		public virtual long? UserId { get; set; }

		[Comment("用户")]
		[TableVisible]
		[ReadOnly(true)]
		[Ignore]
		public virtual long UserName { get; set; }
	}

	public class EventEntityBase : EventEntityBase<long>
	{

	}
}
