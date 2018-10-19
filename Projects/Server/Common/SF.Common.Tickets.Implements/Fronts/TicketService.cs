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

using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Common.Tickets.DataModels;
using SF.Sys.Data;
using SF.Sys.Services;
using SF.Sys.Entities;
using SF.Sys;
using SF.Sys.Clients;
using SF.Sys.Auth;
using SF.Common.Tickets.Management;

namespace SF.Common.Tickets.Front
{

	public class TicketService:
		ITicketService

	{
		public IDataScope DataScope { get; }
		public IServiceInstanceDescriptor ServiceInstanceDescriptor { get; }
		public IAccessToken AccessToken { get; }
		public Lazy<IEntityPropertyFiller> EntityPropertyFiller { get; }
		public Lazy<ITicketManager> TicketManager { get; }
		public Lazy<ITicketReplyManager> TicketReplyManager { get; }
		long EnsureUserIdent()
			=> AccessToken.User.EnsureUserIdent();
		public TicketService(
			IDataScope DataScope, 
			Lazy<IEntityPropertyFiller> EntityPropertyFiller,
			IAccessToken AccessToken,
			Lazy<ITicketManager> TicketManager,
			Lazy<ITicketReplyManager> TicketReplyManager
			)
		{
			this.DataScope = DataScope;
			this.EntityPropertyFiller = EntityPropertyFiller;
			this.AccessToken = AccessToken;
			this.TicketManager = TicketManager;
			this.TicketReplyManager = TicketReplyManager;
		}

		public Task<OptionItem<long>[]> GetCategories()
		{
			return DataScope.Use("获取分类", ctx =>
				 (from c in ctx.Queryable<DataModels.DataTicketCategory>()
				  where c.LogicState == EntityLogicState.Enabled
				  orderby c.Name
				  select new OptionItem<long>
				  {
					  Name = c.Name,
					  Value = c.Id
				  }).ToArrayAsync()
			);
		}

		public async Task<Ticket> GetTicket(long Id)
		{
			var uid = EnsureUserIdent();
			var re = await DataScope.Use("获取工单", ctx =>
				   (from t in ctx.Queryable<DataModels.DataTicket>()
					where t.Id == Id && t.OwnerId == uid && t.LogicState == EntityLogicState.Enabled
					select new Ticket
					{
						Id = t.Id,
						Content = t.Content,
						CreatedTime = t.CreatedTime,
						Name = t.Name,
						State = t.State,
						UpdatedTime = t.UpdatedTime,
						ImageStr=t.Images,
						CategoryId = t.CategoryId,
						CategoryName = t.Category.Name,
						Replies = (from r in t.Replies
									where r.LogicState == EntityLogicState.Enabled
									orderby r.CreatedTime
									select new TicketReply
									{
										Content = r.Content,
										Id = r.Id,
										TicketId=t.Id,
										CreatedTime = r.CreatedTime,
										UpdatedTime = r.UpdatedTime,
										Name = r.Name,
										OwnerId = r.OwnerId.HasValue && r.OwnerId.Value != uid ? r.OwnerId : null,
										ImageStr=r.Images
									}).ToArray()
					}
					).SingleOrDefaultAsync()
			);
			if (re == null)
				throw new PublicArgumentException("找不到指定的工单:"+Id);

			if (re.ImageStr != null)
			{
				re.Images = Json.Parse<TicketImage[]>(re.ImageStr);
				re.ImageStr = null;
			}

			foreach(var r in re.Replies)
				if (r.ImageStr != null)
				{
					r.Images = Json.Parse<TicketImage[]>(r.ImageStr);
					r.ImageStr = null;
				}

			await EntityPropertyFiller.Value.Fill(null, re.Replies.ToArray());

			return re;
		}

		public Task<QueryResult<Ticket>> ListTickets(ListArgument Arg)
		{
			var uid = EnsureUserIdent();
			var re = DataScope.Use("获取工单", ctx =>
			{
				var q =
				 from t in ctx.Queryable<DataModels.DataTicket>()
				 where t.OwnerId == uid && t.LogicState == EntityLogicState.Enabled
				 select t;

				q = q.Filter(Arg.State, t => t.State);
				return (from t in q
						orderby t.CreatedTime descending
						select new Ticket
						{
							Id = t.Id,
							CreatedTime = t.CreatedTime,
							Name = t.Name,
							State = t.State,
							UpdatedTime = t.UpdatedTime,
							CategoryId=t.CategoryId,
							CategoryName=t.Category.Name
						}
					).ToQueryResultAsync(Arg.Paging);
			});
			
			return re;
		}

		public Task<ObjectKey<long>> CreateTicket(TicketCreateArgument Ticket)
		{
			var uid = EnsureUserIdent();
			return TicketManager.Value.CreateAsync(new TicketEditable
			{
				CategoryId = Ticket.CategoryId,
				Images = Ticket.Images,
				Name = Ticket.Name,
				Content = Ticket.Content,
				OwnerId = uid
			});
		}

		

		public async Task RemoveTicket(long TicketId)
		{
			var uid = EnsureUserIdent();
			var t = await TicketManager.Value.LoadForEdit(ObjectKey.From(TicketId));
			if (t == null || t.LogicState!=EntityLogicState.Enabled)
				throw new PublicArgumentException("找不到指定的工单:" + TicketId);
			t.LogicState = EntityLogicState.Deleted;
			await TicketManager.Value.UpdateAsync(t);
		}

		public Task<ObjectKey<long>> CreateReply(TicketReplyCreateArgument Reply)
		{
			var uid = EnsureUserIdent();
			return TicketReplyManager.Value.CreateAsync(new Management.TicketReply
			{
				TicketId=Reply.TicketId,
				Images = Reply.Images,
				Name = Reply.Name,
				Content = Reply.Content,
				OwnerId = uid
			});
		}


		public async Task RemoveReply(long ReplyId)
		{
			var uid = EnsureUserIdent();
			var t = await TicketReplyManager.Value.LoadForEdit(ObjectKey.From(ReplyId));
			if (t == null || t.LogicState != EntityLogicState.Enabled)
				throw new PublicArgumentException("找不到指定的工单回复:" + ReplyId);
			t.LogicState = EntityLogicState.Deleted;
			await TicketReplyManager.Value.UpdateAsync(t);
		}
	}
}
