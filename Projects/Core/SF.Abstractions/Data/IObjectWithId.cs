using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	public interface IObjectWithId<TKey>
		   where TKey : IEquatable<TKey>
	{
		TKey Id { get; }
	}
}
