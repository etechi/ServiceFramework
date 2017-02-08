using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SF.Data.Entity
{

	public interface IQueryArgument<TKey>
	{
		Option<TKey> Id { get; }
	}
}
