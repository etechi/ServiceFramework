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
using SF.Common.Notifications.Models;
using SF.Common.Notifications.Senders;
using SF.Sys.Linq;
using SF.Sys.Reminders;
using SF.Common.Notifications.DataModels;
using SF.Sys.Collections.Generic;

namespace SF.Common.Notifications.Management
{
	public class NotificationManager :
		AutoModifiableEntityManager<
			ObjectKey<long>, 
			Notification, 
			Notification, 
			NotificationQueryArgument,
			NotificationEditable,
			DataModels.DataNotification
			>,
		INotificationManager
	{
		Lazy<IRemindService> RemindService { get; }

		Lazy<IEntityCache<long, NotificationSendPolicy>> Cache { get; }
		TypedInstanceResolver<INotificationSendProvider> NotificationSendProviderResolver { get; }
		public NotificationManager(
			IEntityServiceContext ServiceContext,
			Lazy<IEntityCache<long, NotificationSendPolicy>> Cache,
			Lazy<IRemindService> RemindService,
			TypedInstanceResolver<INotificationSendProvider> NotificationSendProviderResolver
			) : base(ServiceContext)
		{
			this.RemindService = RemindService;
			this.Cache = Cache;
			this.NotificationSendProviderResolver = NotificationSendProviderResolver;
		}

		protected override IContextQueryable<DataModels.DataNotification> OnBuildQuery(IContextQueryable<DataModels.DataNotification> Query, NotificationQueryArgument Arg)
		{
			if (Arg.State.HasValue)
			{
				var now = Now;
				switch (Arg.State.Value)
				{
					case NotificationState.Available:
						Query = Query.Where(n => n.Time <= now && n.Expires > now);
						break;
					case NotificationState.Expired:
						Query = Query.Where(n => n.Expires <= now);
						break;
					case NotificationState.NotStart:
						Query = Query.Where(n => n.Time > now );
						break;
				}
			}
			if (Arg.TargetUserId.HasValue)
			{
				var uid = Arg.TargetUserId.Value;
				Query = Query.Where(n => n.Targets.Any(t => t.UserId == uid));
			}
			return base.OnBuildQuery(Query, Arg);
		}

		public async Task<long> FindPolicy(string Ident)
		{
			var pid=await DataScope.Use("查找策略", DataContext=>
				DataContext
				.Set<DataModels.DataNotificationSendPolicy>()
				.AsQueryable()
				.Where(p => 
					p.LogicState == EntityLogicState.Enabled && 
					p.Ident == Ident
					)
				.Select(p => p.Id)
				.SingleOrDefaultAsync()
				);
			if (pid == 0)
				throw new PublicArgumentException("找不到通知发送策略:" + Ident);
			return pid;
		}

		protected override async Task OnUpdateModel(IModifyContext ctx)
		{
			if(ctx.Editable.Mode==NotificationMode.Normal)
				await UpdateUserStatus(ctx);
			await base.OnUpdateModel(ctx);
		}
		

		private static async Task UpdateUserStatus(IModifyContext ctx)
		{
			var editable = ctx.Editable;
			var model = ctx.Model;

			if (editable.Targets != null)
			{
				var users = new HashSet<long>(editable.Targets);
				
				var statuses = await ctx.DataContext
					.Queryable<DataModels.DataNotificationUserStatus>()
					.Where(s => users.Contains(s.Id)).ToDictionaryAsync(s => s.Id);
				if (model.Targets == null && ctx.Action == ModifyAction.Update)
					model.Targets = await ctx.DataContext
					.Queryable<DataModels.DataNotificationTarget>()
					.Where(t => t.NotificationId == ctx.Model.Id)
					.ToListAsync();

				if(model.Targets!=null)
					foreach (var t in model.Targets.Where(t => !users.Contains(t.UserId)))
						if (statuses.TryGetValue(t.UserId, out var s))
						{
							if (s.Received > 0)
								s.Received--;
							if (s.ReceivedUnreaded > 0 && !t.ReadTime.HasValue)
								s.ReceivedUnreaded--;
							ctx.DataContext.Update(s);
						}
				foreach (var u in users.Where(i => model.Targets==null || !model.Targets.Any(t => t.UserId == i)))
					if (statuses.TryGetValue(u, out var s))
					{
						s.Received++;
						s.ReceivedUnreaded++;
						ctx.DataContext.Update(s);
					}
					else
						ctx.DataContext.Add(
							new DataNotificationUserStatus
							{
								Id = u,
								Received = 1,
								ReceivedUnreaded = 1
							});

				
			}
		}

		protected override async Task OnNewModel(IModifyContext ctx)
		{
			var model = ctx.Model;
			var editable = ctx.Editable;
			if (editable.Time == default)
				editable.Time = Now;

			//默认过期事件为10分钟
			if (editable.Expires==default)
				editable.Expires = editable.Time.AddMinutes(10);

			var sas = editable.PolicyId.HasValue?
					await Cache.Value.Find(editable.PolicyId.Value):
					null;
			if (sas != null)
			{
				if (editable.Name.IsNullOrEmpty())
					editable.Name = 
						sas.NameTemplate.HasContent()?
						sas.NameTemplate.Replace(editable.Args):
						sas.Name;

				if (editable.Content.IsNullOrEmpty() && sas.ContentTemplate.HasContent())
					editable.Content = sas.ContentTemplate.Replace(editable.Args);
			}

			await base.OnNewModel(ctx);


			if ((sas?.Actions?.Length??0) == 0)
				return;

			if (editable.Targets == null)
				await AddSendRecord(ctx.DataContext,model.Id, sas, editable, null);
			else
				foreach (var target in editable.Targets)
					await AddSendRecord(ctx.DataContext, model.Id, sas, editable, target);

			await RemindService.Value.Setup(
				new RemindSetupArgument
				{
					BizIdent = model.Id,
					BizIdentType = "Notification",
					BizType = "发送通知",
					Name = editable.Name,
					RemindableName = typeof(Remindable).FullName,
					RemindTime = editable.Time,
					UserId = editable?.Targets?.FirstOrDefault() ?? editable.SenderId
				});
		}
		protected override async Task OnRemoveModel(IModifyContext ctx)
		{
			await RemoveStatus(ctx);

			await RemindService.Value.Remove(
				"发送通知",
				"Notification",
				ctx.Model.Id
				);
			await base.OnRemoveModel(ctx);
		}

		private static async Task RemoveStatus(IModifyContext ctx)
		{
			var model = ctx.Model;
			if (model.Mode == NotificationMode.Normal)
			{
				//全体通知数量不记录
				var dics = await ctx.DataContext
					.Queryable<DataModels.DataNotificationTarget>()
					.Where(t => t.NotificationId == model.Id)
					.ToDictionaryAsync(t => t.UserId);
				var users = dics.Keys.ToArray();

				var statuses = await ctx.DataContext
					.Queryable<DataModels.DataNotificationUserStatus>(false)
					.Where(s => users.Contains(s.Id))
					.ToArrayAsync();
				foreach (var status in statuses)
				{
					if (status.Received > 0)
						status.Received--;
					if (!dics[status.Id].ReadTime.HasValue && status.ReceivedUnreaded > 0)
						status.ReceivedUnreaded--;
					ctx.DataContext.Update(status);
				}
			}
		}

		async Task AddSendRecord(
			IDataContext DataContext,
			long NotificationId,
			NotificationSendPolicy nsp,
			NotificationEditable editable,
			long? target
			)
		{
			foreach (var sa in nsp.Actions)
			{
				var sendRecord = new DataModels.DataNotificationSendRecord
					{
						BizIdent = editable.BizIdent,
						Expires = editable.Expires,
						RetryLimit = sa.RetryLimit,
						TargetId = target,
						SendTime = editable.Time,
						RetryInterval = sa.RetryInterval,
						Template = sa.ExtTemplateId,
						Content = sa.ContentTemplate.Replace(editable.Args),
						Args = (sa.ExtTemplateArguments?.Length??0)==0?
							null:
							Json.Stringify(
								sa.ExtTemplateArguments.ToDictionary(
									a=>a.Name,
									a=>a.Value.Replace(editable.Args)
								)
							),
						Id = await IdentGenerator.GenerateAsync<DataModels.DataNotificationSendRecord>(),
						NotificationId = NotificationId,
						ProviderId = sa.ProviderId,
						Time = Now,
						Status = SendStatus.Sending,
						Target = sa.TargetTemplate.Replace(editable.Args),
						Title = sa.TitleTemplate.Replace(editable.Args),
						UserId = target
					};
					if (sendRecord.Target.IsNullOrEmpty())
					{
						if (target.HasValue)
						{
							var p = NotificationSendProviderResolver(sa.ProviderId);
							var re= await p.TargetResolve(new[] { target.Value });
							sendRecord.Target = re.SingleOrDefault();
						}
						if (sendRecord.Target.IsNullOrEmpty())
						{
							sendRecord.Error = $"无法获取通知发送目标：发送服务:{sa.ProviderId} {editable.Name}";
							sendRecord.Status = SendStatus.Failed;
						}
					}
					DataContext.Add(sendRecord);
				}
		}
	}

}
