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

    public abstract class UIObjectEntityBase<K> : ObjectEntityBase<K>,IUIObjectEntity
		where K:IEquatable<K>
	{
		[Comment("标题", "用于UI显示")]
		[MaxLength(100)]
		[Required]
		public virtual string Title { get; set; }

		[Comment("副标题", "用于UI显示")]
		[MaxLength(100)]
		public virtual string SubTitle { get; set; }

		[Comment("提示", "用于UI显示")]
		[MaxLength(100)]
		public virtual string Remarks { get; set; }

		[Comment("说明", "用于UI显示")]
		[MaxLength(200)]
		public virtual string Description { get; set; }

		[Comment("备注", "内部使用，不用于UI显示")]
		[MaxLength(200)]
		public virtual string Memo { get; set; }

		[Comment("图片","大尺寸图片")]
		[MaxLength(100)]
		public virtual string Image { get; set; }

		[Comment("图标", "小尺寸图片")]
		[MaxLength(100)]
		public virtual string Icon { get; set; }

	}

	public class UIObjectEntityBase : UIObjectEntityBase<long>
	{

	}
}
