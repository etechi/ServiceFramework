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

namespace SF.Common.Tickets
{

	public class TicketService:
		ITicketService

	{
		public IDataScope DataScope { get; }
		public IServiceInstanceDescriptor ServiceInstanceDescriptor { get; }
		public TicketService(IDataScope DataScope, IServiceInstanceDescriptor ServiceInstanceDescriptor)
		{
			this.DataScope = DataScope;
			this.ServiceInstanceDescriptor = ServiceInstanceDescriptor;
		}

		public Task<OptionItem<long>[]> GetCategories()
		{
			throw new NotImplementedException();
		}

		public Task<Ticket> GetTicket(long Id)
		{
			throw new NotImplementedException();
		}

		public Task<QueryResult<Ticket>> ListTickets(ListArgument Arg)
		{
			throw new NotImplementedException();
		}

		public Task<ObjectKey<long>> CreateTicket(Ticket Ticket)
		{
			throw new NotImplementedException();
		}

		public Task<ObjectKey<long>> UpdateTicket(Ticket Ticket)
		{
			throw new NotImplementedException();
		}

		public Task RemoveTicket(long TicketId)
		{
			throw new NotImplementedException();
		}

		public Task<ObjectKey<long>> CreateReply(TicketReply Reply)
		{
			throw new NotImplementedException();
		}

		public Task<ObjectKey<long>> UpdateReply(TicketReply Reply)
		{
			throw new NotImplementedException();
		}

		public Task ReplyTicket(long ReplyId)
		{
			throw new NotImplementedException();
		}
	}
}
