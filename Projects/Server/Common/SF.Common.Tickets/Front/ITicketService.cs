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


using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.Threading.Tasks;

namespace SF.Common.Tickets.Front
{
	public class ListArgument : PagingArgument
	{
		/// <summary>
		/// 状态
		/// </summary>
		public TicketState? State { get; set; }
	}


	/// <summary>
	/// 工单服务
	/// </summary>
	[NetworkService]
	[AnonymousAllowed]
	public interface ITicketService
	{
		/// <summary>
		/// 获取工单目录
		/// </summary>
		/// <returns>目录列表</returns>
		Task<OptionItem<long>[]> GetCategories();


		/// <summary>
		/// 通过ID获取工单
		/// </summary>
		/// <param name="Id">主键</param>
		/// <returns>工单</returns>
		Task<Ticket> GetTicket(long Id);

		/// <summary>
		/// 获取工单列表
		/// </summary>
		/// <param name="Arg">工单列表参数</param>
		/// <returns>工单列表</returns>
		Task<QueryResult<Ticket>> ListTickets(ListArgument Arg);

		/// <summary>
		/// 新建工单
		/// </summary>
		/// <param name="Ticket">工单参数</param>
		/// <returns></returns>
		Task<ObjectKey<long>> CreateTicket(TicketCreateArgument Ticket);

		/// <summary>
		/// 删除工单
		/// </summary>
		/// <param name="TicketId">工单ID</param>
		/// <returns></returns>
		Task RemoveTicket(long TicketId);

		/// <summary>
		/// 新建回复
		/// </summary>
		/// <param name="Reply">回复参数</param>
		/// <returns></returns>
		Task<ObjectKey<long>> CreateReply(TicketReplyCreateArgument Reply);

		/// <summary>
		/// 删除回复
		/// </summary>
		/// <param name="ReplyId">回复ID</param>
		/// <returns></returns>
		Task RemoveReply(long ReplyId);
	}

}
