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
		IEntityWithId<T>,
		IEquatable<ObjectKey<T>>,
		IComparable<ObjectKey<T>>
		 where T:IEquatable<T>

	{
		[Key]
		[Comment("标识")]
		[ReadOnly(true)]
		[TableVisible]
		public T Id { get; set; }

		public int CompareTo(ObjectKey<T> other)
		{
			return Comparer<T>.Default.Compare(Id, other.Id);
		}

		public bool Equals(ObjectKey<T> other)
		{
			return EqualityComparer<T>.Default.Equals(Id, other.Id);
		}
		public override bool Equals(object obj)
		{
			var ok = obj as ObjectKey<T>;
			return ok == null ? false : Equals(ok);
		}
		public override int GetHashCode()
		{
			return EqualityComparer<T>.Default.GetHashCode(Id);
		}

		public override string ToString()
		{
			return EqualityComparer<T>.Default.Equals(Id, default(T)) ? null : Id.ToString();
		}

		//public static implicit  operator ObjectKey<long>(long id)=>new ObjectKey<long> { Id=id};
	}
	
}
