using SF.Core.ServiceManagement;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SF.Entities
{
	public interface IQueryArgument<TKey>
	{
		Option<TKey> Id { get; }
	}
	public class QueryArgument<TKey> :
		IQueryArgument<TKey>
	{
		[Comment("ID")]
		public Option<TKey> Id { get; set; }

	}
	public class QueryArgument :
		QueryArgument<long>
	{
	}
}
