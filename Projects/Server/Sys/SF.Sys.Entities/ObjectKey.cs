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
			return EqualityComparer<T>.Default.Equals(Id, default(T)) ? null : Id.ToString();
		}

		
	}
	public static class ObjectKey
	{
		public static ObjectKey<T> From<T>(T Id)
			where T : IEquatable<T>
		{
			return new ObjectKey<T> { Id = Id };
		}
	}
}
