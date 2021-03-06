﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Linq;
using SF.Sys.Reminders;
using SF.Sys.Services;
using SF.Sys.TimeServices;

namespace SF.Common.Notifications.Senders
{
	public class NotificationSendActionSource :
		IReminderActionSource
	{
		public string Name => "通知发送";

		IDataScope DataScope { get; }
		TypedInstanceResolver<INotificationSendProvider> NotificationSendProviderResolver { get; }
		ITimeService TimeService { get; }
		
		public NotificationSendActionSource(
			ITimeService TimeService,
			IDataScope DataScope,
			TypedInstanceResolver<INotificationSendProvider> NotificationSendProviderResolver
			)
		{
			this.TimeService = TimeService;
			this.DataScope = DataScope;
			this.NotificationSendProviderResolver = NotificationSendProviderResolver;
		}

		class SendAction : IRemindAction
		{
			public TypedInstanceResolver<INotificationSendProvider> NotificationSendProviderResolver { get; set; }
			public DataModels.DataNotificationSendRecord SendRecord { get; set; }
			public IDataScope DataScope { get; set; }
			public DateTime Time => SendRecord.SendTime;
			public DateTime Now { get; set; }
			public string BizIdent => SendRecord.BizIdent;

			public Task Execute(long RecordId, bool RetryMode)
			{
				 return DataScope.Use("执行通知", async DataContext =>
				 {
					 try
					 {
						 var provider = NotificationSendProviderResolver(SendRecord.ProviderId);
						 SendRecord.SendCount++;
						 var result = await provider.Send(SendRecord);
						 SendRecord.Result = result.Limit(1000);
						 SendRecord.Status = Models.SendStatus.Success;
						 SendRecord.LastSendTime = Now;
						 DataContext.Update(SendRecord);
						 await DataContext.SaveChangesAsync();
					 }
					 catch (Exception ex)
					 {
						 SendRecord.Error = ex.Message.Limit(1000);
						 var nextTime = Now.AddSeconds(SendRecord.RetryInterval);
						 if (nextTime < SendRecord.Expires && (SendRecord.RetryLimit == 0 || SendRecord.SendCount < SendRecord.RetryLimit))
							 SendRecord.SendTime = nextTime;
						 else
							 SendRecord.Status = Models.SendStatus.Failed;
						 DataContext.Update(SendRecord);
						 await DataContext.SaveChangesAsync();
					 }
				 });
			}

			public Task<RemindActionInfo> GetInfo()
			{
				return Task.FromResult(
					new RemindActionInfo
					{
						Name=SendRecord.Title,
						Action="发送通知"						
					});
			}
		}
		public async Task<IEnumerable<IRemindAction>> GetActions(long UserId, DateTime Time)
		{
			var now = TimeService.Now;
			var re=await DataScope.Use("查询动作", async DataContext =>
			{
				var q = from r in DataContext.Set<DataModels.DataNotificationSendRecord>().AsQueryable()
						where r.UserId == UserId &&
								r.Status == Models.SendStatus.Sending &&
								r.SendTime <= now
						select r;

				var ire = await q.Take(20).ToArrayAsync();

				if (ire.Length == 0)
					Enumerable.Empty<IRemindAction>();

				var hasExpired = false;
				foreach (var r in ire.Where(r => r.Expires <= now || r.RetryLimit > 0 && r.SendCount >= r.RetryLimit))
				{
					r.Status = Models.SendStatus.Failed;
					DataContext.Update(r);
					hasExpired = true;
				}
				if (hasExpired)
					await DataContext.SaveChangesAsync();
				return ire;
			});
			return re.Where(r => r.Expires > now)
				.Select(r => new SendAction
				{
					NotificationSendProviderResolver = NotificationSendProviderResolver,
					SendRecord = r,
					DataScope=DataScope
				});
		}

	}

}
