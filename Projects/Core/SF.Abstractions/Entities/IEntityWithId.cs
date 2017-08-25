using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Entities
{
	public interface IEntityWithId<TKey>
		   where TKey : IEquatable<TKey>
	{
		TKey Id { get; }
	}
	public interface IEntityWithKey
	{
		string Key { get;  }
	}
}
