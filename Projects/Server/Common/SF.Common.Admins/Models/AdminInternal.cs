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


using SF.Auth.IdentityServices.Models;
using SF.Sys.Annotations;
using SF.Sys.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Admins.Models
{
	/// <summary>
	/// 管理员
	/// </summary>
	[EntityObject]
	public class AdminInternal : ObjectEntityBase
	{      
		/// <summary>
		/// 账号
		/// </summary>
		/// <remarks>账号用于登录，不能重复,不能修改</remarks>
		[MaxLength(50)]
		[Required]
		[TableVisible]
		[Uneditable]
		public string UserName { get; set; }

		/// <summary>
		/// 角色
		/// </summary>
		[TableVisible]
		[Ignore]
		public string RoleNames { get; set; }
		/// <summary>
		/// 图标
		/// </summary>
		[MaxLength(100)]
		[Image]
		public virtual string Icon { get; set; }

		/// <summary>
		/// 头像
		/// </summary>
		[MaxLength(100)]
		[Image]
		public virtual string Image { get; set; }

		///// <summary>
		///// 登录次数
		///// </summary>
		//[TableVisible]
		//public int SigninCount { get; set; }

		///// <summary>
		///// 最后登录
		///// </summary>
		//[TableVisible]
		//public DateTime? LastSigninTime { get; set; }

		///// <summary>
		///// 最后地址
		///// </summary>
		//[TableVisible]
		//public string LastAddress { get; set; }

		///// <summary>
		///// 最后设备
		///// </summary>
		//[TableVisible]
		//public SF.Sys.Clients.ClientDeviceType LastDevice { get; set; }


	}
}

