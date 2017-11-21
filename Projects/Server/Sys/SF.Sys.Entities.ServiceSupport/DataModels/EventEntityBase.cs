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

using SF.Sys.Data;
using SF.Sys.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SF.Entities.DataModels
{

	public abstract class EventEntityBase<K> : IEntityWithId<K>, IEventEntity
		where K:IEquatable<K>
	{
		/// <summary>
		/// ID
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public virtual K Id { get; set; }

		/// <summary>
		/// 区域
		/// </summary>
		[Index]
		public virtual long? ScopeId { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[Index]
		public virtual long? UserId { get; set; }

		/// <summary>
		/// 时间
		/// </summary>
		[Index]
		public virtual DateTime Time { get; set; }

	}

	public class EventEntityBase : EventEntityBase<long>
	{

	}
}
