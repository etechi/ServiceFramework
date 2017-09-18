using SF.Core.ServiceManagement;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
	public class ObjectQueryArgument<TKey> : QueryArgument<TKey>
	{
		[Comment("名称")]
		[MaxLength(100)]
		public string Name { get; set; }

		[Comment("逻辑状态")]
		public EntityLogicState? LogicState { get; set; }

	}
}
