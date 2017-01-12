using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SF.Data.Services
{

	public interface IQueryArgument<TKey>
	{
		Option<TKey> Id { get; }
	}
}
