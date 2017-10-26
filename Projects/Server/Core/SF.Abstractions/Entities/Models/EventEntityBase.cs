﻿#region Apache License Version 2.0
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

using SF.Auth.Users;
using SF.Auth.Users.Models;
using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SF.Data.Models
{


    public abstract class EventEntityBase<K> : IEntityWithId<K>, IEventEntity
		where K:IEquatable<K>
	{
		[Comment("Id")]
		[Key]
		[ReadOnly(true)]
		[TableVisible]
		public virtual K Id { get; set; }

	
		[Comment("时间")]
		[TableVisible]
		[ReadOnly(true)]
		public virtual DateTime Time{ get; set; }

		[Comment("用户")]
		[ReadOnly(true)]
		[EntityIdent(typeof(User),  nameof(UserName))]
		public virtual long? UserId { get; set; }

		[Comment("用户")]
		[TableVisible]
		[ReadOnly(true)]
		[Ignore]
		public virtual string UserName { get; set; }
	}

	public class EventEntityBase : EventEntityBase<long>
	{

	}
}
