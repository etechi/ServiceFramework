using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Entities
{
	public class ObjectKey<T>:
		IEntityWithId<T>
		 where T:IEquatable<T>

	{
		[Key]
		[Comment("标识")]
		[ReadOnly(true)]
		[TableVisible]
		public T Id { get; set; }

		public override string ToString()
		{
			return EqualityComparer<T>.Default.Equals(Id, default(T)) ? null : Id.ToString();
		}

		//public static implicit  operator ObjectKey<long>(long id)=>new ObjectKey<long> { Id=id};
	}
	
}
