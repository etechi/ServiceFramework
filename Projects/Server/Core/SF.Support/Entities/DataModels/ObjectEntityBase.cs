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

using SF.Data;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF.Entities.DataModels
{

    public abstract class ObjectEntityBase<K> : 
		IEntityWithId<K>, 
		IObjectEntity, 
		IEntityWithScope, 
		IEntityWithLogicState
		where K:IEquatable<K>
	{

		[Comment("Id")]
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public virtual K Id { get; set; }

		[Comment("名称")]
		[MaxLength(100)]
		[Index]
		[Required]
		public virtual string Name { get; set; }

		[Comment("区域")]
		[Index]
		[ServiceScopeId]
		public virtual long? ScopeId { get; set; }

		[Comment("对象状态")]
		public virtual EntityLogicState LogicState { get; set; }

		[Comment("所有人")]
		[Index]
		public virtual long OwnerId { get; set; }

		[Comment("创建时间")]
		[Index]
		public virtual DateTime CreatedTime { get; set; }

		
		[Comment("修改人")]
		[Index]
		public virtual long UpdatorId { get; set; }

		[Comment("修改时间")]
		public virtual DateTime UpdatedTime { get; set; }

		[Comment("内部备注")]
		public virtual string InternalRemarks { get; set; }
	
		[ConcurrencyCheck]
		[Timestamp]
		[Comment(Name = "乐观锁时间戳")]
		public virtual byte[] TimeStamp { get; set; }

	}

	public class ObjectEntityBase : ObjectEntityBase<long>
	{

	}
}
