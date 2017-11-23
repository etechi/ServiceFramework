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

using SF.Sys.Entities;
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

    public abstract class UIObjectEntityBase<K> : ObjectEntityBase<K>,IUIObjectEntity
		where K:IEquatable<K>
	{
		///<title>标题</title>
		/// <summary>
		/// 用于UI显示
		/// </summary>
		[MaxLength(100)]
		[Required]
		public virtual string Title { get; set; }

		///<title>副标题</title>
		/// <summary>
		/// 用于UI显示
		/// </summary>
		[MaxLength(100)]
		public virtual string SubTitle { get; set; }

		///<title>提示</title>
		/// <summary>
		/// 用于UI显示
		/// </summary>
		[MaxLength(100)]
		public virtual string Remarks { get; set; }

		///<title>说明</title>
		/// <summary>
		/// 用于UI显示
		/// </summary>
		[MaxLength(200)]
		public virtual string Description { get; set; }

		///<title>备注</title>
		/// <summary>
		/// 内部使用，不用于UI显示
		/// </summary>
		[MaxLength(200)]
		public virtual string Memo { get; set; }

		///<title>图片</title>
		/// <summary>
		/// 用于UI显示
		/// </summary>
		[MaxLength(100)]
		public virtual string Image { get; set; }

		///<title>图标</title>
		/// <summary>
		/// 用于UI显示
		/// </summary>
		[MaxLength(100)]
		public virtual string Icon { get; set; }

	}

	public class UIObjectEntityBase : UIObjectEntityBase<long>
	{

	}
}
