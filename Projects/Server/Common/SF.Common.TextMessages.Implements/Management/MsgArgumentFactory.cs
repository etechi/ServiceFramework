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

using SF.Sys;
using SF.Sys.Entities;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Data;
using SF.Sys.Services;
using System;
using System.Collections.Generic;
using SF.Common.TextMessages.Models;
using SF.Sys.Events;

namespace SF.Common.TextMessages.Management
{
	public class MsgArgumentFactory : IMsgArgumentFactory
	{
		System.Collections.Concurrent.ConcurrentDictionary<long, string> PolicyIdentes { get; } =
			new System.Collections.Concurrent.ConcurrentDictionary<long, string>();
		System.Collections.Concurrent.ConcurrentDictionary<string, MsgSendArgument[]> Policies { get; } = 
			new System.Collections.Concurrent.ConcurrentDictionary<string, MsgSendArgument[]>();
		IScoped<IDataSet<DataModels.MsgPolicy>> ScopedPolicies { get; }
		public MsgArgumentFactory(
			IScoped<IDataSet<DataModels.MsgPolicy>> ScopedPolicies, 
			IEventSubscriber<EntityChanged<DataModels.MsgPolicy>> OnPolicyModified
			)
		{
			OnPolicyModified.Wait(ei =>
			{
				if(PolicyIdentes.TryRemove(ei.Id,out var ident))
					Policies.TryRemove(ident,out var _);
				return Task.CompletedTask;
			});
			this.ScopedPolicies = ScopedPolicies;
		}
		async Task<MsgSendArgument[]> LoadPolicy(string Id)
		{
			if (Policies.TryGetValue(Id, out var mp))
				return mp;
			return await ScopedPolicies.Use(async ps =>
			{
				var re = await ps.AsQueryable()
					.Where(p =>
						p.Ident == Id &&
						p.LogicState == EntityLogicState.Enabled
						)
					.Select(p => new { Id = p.Id, Actions = p.Actions })
					.SingleOrDefaultAsync();
				if (re == null) return null;
				var sas = Json.Parse<MsgSendArgument[]>(re.Actions);
				PolicyIdentes.TryAdd(re.Id, Id);
				return Policies.GetOrAdd(Id, sas);
			});
		}
		public async Task<MsgSendArgument[]> Create(long? TargetId, Message Message)
		{
			var sas = await LoadPolicy(Message.Policy);
			if (sas == null)
				return Array.Empty<MsgSendArgument>();
			return sas.Select(a =>
				new MsgSendArgument
				{
					Title = a.Title.Replace(Message.Arguments),
					Target = a.Target.Replace(Message.Arguments),
					Content = a.Content.Replace(Message.Arguments),
					Arguments = a.Arguments.Select(p => (p.Key, p.Value.Replace(Message.Arguments))).ToArray(),
					MsgProviderId = a.MsgProviderId,
					TargetId = TargetId
				}).ToArray();

		}
	}

}
