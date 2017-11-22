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
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Entities.Annotations;
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

		/// <summary>
		/// Id
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public virtual K Id { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[MaxLength(100)]
		[Index]
		[Required]
		public virtual string Name { get; set; }

		/// <summary>
		/// 区域
		/// </summary>
		[Index]
		[ServiceScopeId]
		public virtual long? ServiceDataScopeId { get; set; }

		/// <summary>
		/// 对象状态
		/// </summary>
		public virtual EntityLogicState LogicState { get; set; }

		/// <summary>
		/// 所有人
		/// </summary>
		[Index]
		public virtual long OwnerId { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[Index]
		[CreatedTime]
		public virtual DateTime CreatedTime { get; set; }


		/// <summary>
		/// 修改人
		/// </summary>
		[Index]
		public virtual long UpdatorId { get; set; }

		/// <summary>
		/// 修改时间
		/// </summary>
		[UpdatedTime]
		public virtual DateTime UpdatedTime { get; set; }

		/// <summary>
		/// 内部备注
		/// </summary>
		public virtual string InternalRemarks { get; set; }

		/// <summary>
		/// 乐观锁时间戳
		/// </summary>
		[ConcurrencyCheck]
		[Timestamp]
		
		public virtual byte[] TimeStamp { get; set; }

	}

	public class ObjectEntityBase : ObjectEntityBase<long>
	{

	}
}
