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

using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Tickets.Management
{
	public class TicketQueryArguments : ObjectQueryArgument
	{
	
		/// <summary>
		/// 工单分类
		/// </summary>
		[EntityIdent(typeof(TicketCategory))]
		public long? CategoryId { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[EntityIdent(typeof(User))]
		public long? OwnerId { get; set; }

		/// <summary>
		/// 发布日期
		/// </summary>
		public NullableDateQueryRange CreateTime { get; set; }

		/// <summary>
		/// 工单状态
		/// </summary>
		public TicketState? State { get; set; }

	}

	/// <summary>
	/// 工单管理
	/// </summary>
	/// <typeparam name="TInternal"></typeparam>
	/// <typeparam name="TEditable"></typeparam>
	[NetworkService]
	[EntityManager]
	[DefaultAuthorize(PredefinedRoles.客服专员, true)]
	[DefaultAuthorize(PredefinedRoles.运营专员)]
	[DefaultAuthorize(PredefinedRoles.系统管理员)]
	public interface ITicketManager<TInternal, TEditable> :
		IEntitySource<ObjectKey<long>, TInternal, TicketQueryArguments>,
		IEntityManager<ObjectKey<long>, TEditable>
		where TInternal : TicketInternal
		where TEditable : TicketEditable
	{
	}
	public interface ITicketManager:
		ITicketManager<TicketInternal,TicketEditable>
	{

	}

}
