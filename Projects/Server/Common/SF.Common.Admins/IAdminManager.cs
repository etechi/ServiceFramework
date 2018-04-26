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
using SF.Common.Admins.Models;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Common.Admins
{
	public class AdminQueryArgument : ObjectQueryArgument
	{
		/// <summary>
		/// 用户名
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 角色
		/// </summary>
		[EntityIdent(typeof(Role))]
		public string RoleId { get; set; }
	}

	/// <summary>
	/// 管理员设置
	/// </summary>
	public class AdminSetting
	{
		/// <summary>
		/// ID
		/// </summary>
		[Key]
		[ReadOnly(true)]
		public long Id { get; set; }

		/// <summary>
		/// 称呼
		/// </summary>
		[Required]
		[MaxLength(100)]
		public string Name { get; set; }
		///<title>密码</title>
		/// <summary>
		/// 设置后可修改密码
		/// </summary>
		[Password]
		[MaxLength(100)]
		public string Password { get; set; }
		/// <summary>
		/// 图标
		/// </summary>
		[Image]
		[MaxLength(100)]
		public string Icon { get; set; }
		/// <summary>
		/// 图片
		/// </summary>
		[Image]
		[MaxLength(100)]
		public string Image { get; set; }
	}
	/// <summary>
	/// 系统管理员
	/// </summary>
	/// <typeparam name="TInternal"></typeparam>
	/// <typeparam name="TEditable"></typeparam>
	/// <typeparam name="TQueryArgument"></typeparam>
	[EntityManager]
	[NetworkService]
	[DefaultAuthorize(PredefinedRoles.安全专员)]
	[DefaultAuthorize(PredefinedRoles.系统管理员)]
	//[Category("系统管理", "系统管理员管理")]
	public interface IAdminManager<TInternal,TEditable,TQueryArgument> :
		IEntitySource<ObjectKey<long>,TInternal,TQueryArgument>,
		IEntityManager<ObjectKey<long>, TEditable>
		where TInternal:AdminInternal
		where TEditable:AdminEditable
		where TQueryArgument:AdminQueryArgument
	{
		/// <summary>
		/// 获取当前管理员信息
		/// </summary>
		/// <returns>管理员信息</returns>
		[DefaultAuthorize]
		Task<AdminSetting> GetSetting();
		/// <summary>
		/// 设置当前管理员信息
		/// </summary>
		/// <param name="Setting">设置</param>
		[DefaultAuthorize]
		Task SetSetting(AdminSetting Setting);
	}
	public interface IAdminManager:
		IAdminManager<AdminInternal, AdminEditable, AdminQueryArgument>
	{
	}
}

