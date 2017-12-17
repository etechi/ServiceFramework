#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Sys.Annotations;
using SF.Sys.Entities.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Entities
{
	public class ObjectKey<T>:
		IEntityWithId<T>,
		IEquatable<ObjectKey<T>>,
		IComparable<ObjectKey<T>>
		 where T:IEquatable<T>

	{
		/// <summary>
		/// 标识
		/// </summary>
		[Key]
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
			if (!(obj is ObjectKey<T>))
				return false;
			return Equals((ObjectKey<T>)obj);
		}
		public override int GetHashCode()
		{
			return EqualityComparer<T>.Default.GetHashCode(Id);
		}

		public override string ToString()
		{
			return EqualityComparer<T>.Default.Equals(Id, default(T)) ? "" : Id.ToString();
		}
	}
	public class ObjectKey<T1,T2> :
		IEquatable<ObjectKey<T1,T2>>,
		IComparable<ObjectKey<T1,T2>>
		 where T1 : IEquatable<T1>
		where T2 : IEquatable<T2>

	{
		/// <summary>
		/// 标识1
		/// </summary>
		[Key]
		[TableVisible]
		public T1 Id1 { get; set; }

		/// <summary>
		/// 标识2
		/// </summary>
		[Key]
		[TableVisible]
		public T2 Id2 { get; set; }

		public int CompareTo(ObjectKey<T1,T2> other)
		{
			var re=Comparer<T1>.Default.Compare(Id1, other.Id1);
			if (re != 0) return re;
			return Comparer<T2>.Default.Compare(Id2, other.Id2);
		}

		public bool Equals(ObjectKey<T1,T2> other)
		{
			return EqualityComparer<T1>.Default.Equals(Id1, other.Id1) &&
				EqualityComparer<T2>.Default.Equals(Id2, other.Id2)
				;
		}
		public override bool Equals(object obj)
		{
			if (!(obj is ObjectKey<T1,T2>))
				return false;
			return Equals((ObjectKey<T1,T2>)obj);
		}
		public override int GetHashCode()
		{
			return EqualityComparer<T1>.Default.GetHashCode(Id1) ^
				EqualityComparer<T2>.Default.GetHashCode(Id2);
		}

		public override string ToString()
		{
			return Convert.ToString(Id1)+"/"+ Convert.ToString(Id2);
		}
	}
	public static class ObjectKey
	{
		public static ObjectKey<T> From<T>(T Id)
			where T : IEquatable<T>
		{
			return new ObjectKey<T> { Id = Id };
		}
		public static ObjectKey<T1,T2> From<T1,T2>(T1 Id1,T2 Id2)
			where T1 : IEquatable<T1>
			where T2 : IEquatable<T2>
		{
			return new ObjectKey<T1, T2> { Id1 = Id1, Id2 = Id2 };
		}
	}
}
