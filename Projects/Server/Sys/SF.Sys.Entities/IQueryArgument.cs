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

using SF.Core.ServiceManagement;
using SF.Sys.Entities.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SF.Sys.Entities
{
	[AttributeUsage(AttributeTargets.Property)]
	public class StringContainsAttribute : Attribute
	{

	}
	public interface IQueryArgument { }
	public interface IQueryArgument<TKey>: IQueryArgument
	{
		TKey Id { get; }
	}
	public class QueryArgument<TKey> :
		IQueryArgument<TKey>
	{
		public TKey Id { get; set; }

	}


	public class QueryArgument :
		QueryArgument<ObjectKey<long>>
	{
	}
	public class ObjectQueryArgument<TKey> : QueryArgument<TKey>
	{
		/// <summary>
		/// 名称
		/// </summary>
		[MaxLength(100)]
		[StringContains]
		public virtual string Name { get; set; }

		/// <summary>
		/// 逻辑状态
		/// </summary>
		public virtual EntityLogicState? LogicState { get; set; }

	}
}
