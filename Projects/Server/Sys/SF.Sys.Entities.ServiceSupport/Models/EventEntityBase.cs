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
using SF.Sys.Auth;
using SF.Sys.Entities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace SF.Sys.Entities.Models
{


	public abstract class EventEntityBase<K> : IEntityWithId<K>, IEventEntity
		where K:IEquatable<K>
	{
		/// <summary>
		/// Id
		/// </summary>
		[Key]
		[ReadOnly(true)]
		[TableVisible]
		public virtual K Id { get; set; }


		/// <summary>
		/// 时间
		/// </summary>
		[TableVisible]
		[ReadOnly(true)]
		public virtual DateTime Time{ get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[ReadOnly(true)]
		[EntityIdent(typeof(User),  nameof(UserName))]
		public virtual long? UserId { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[TableVisible]
		[ReadOnly(true)]
		[Ignore]
		public virtual string UserName { get; set; }
	}

	public class EventEntityBase : EventEntityBase<long>
	{

	}
}
