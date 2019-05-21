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

namespace SF.Sys.Entities.DataModels
{

    public abstract class DataEntityBase<K> : 
		IEntityWithId<K>
		where K:IEquatable<K>
	{

		/// <summary>
		/// Id
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public virtual K Id { get; set; }


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

	public class DataEntityBase : DataEntityBase<long>
	{

	}
}
