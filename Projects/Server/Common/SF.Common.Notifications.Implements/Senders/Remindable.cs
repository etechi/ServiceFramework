
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Linq;
using SF.Sys.Logging;
using SF.Sys.Reminders;
using SF.Sys.Services;
using SF.Sys.TimeServices;

namespace SF.Common.Notifications.Senders
{
	public class Remindable :
		IRemindable
	{
		public string Name => "通知发送";

		IDataScope DataScope { get; }
		TypedInstanceResolver<INotificationSendProvider> NotificationSendProviderResolver { get; }
		ITimeService TimeService { get; }
		ILogger Logger { get; }
		public Remindable(
			ITimeService TimeService,
			IDataScope DataScope,
			TypedInstanceResolver<INotificationSendProvider> NotificationSendProviderResolver,
			ILogger<Remindable> Logger
			)
		{
			this.Logger = Logger;
			this.TimeService = TimeService;
			this.DataScope = DataScope;
			this.NotificationSendProviderResolver = NotificationSendProviderResolver;
		}

	
		public async Task Remind(IRemindContext Context)
		{
			var recs = await DataScope.Use("执行通知", async DataContext =>
			  {
				  var q = from r in DataContext.Set<DataModels.DataNotificationSendRecord>().AsQueryable()
						  where r.NotificationId == Context.BizIdent &&
								  r.Status == Models.SendStatus.Sending
						  select r;
				  var irecs = await q.ToListAsync();

				  var hasExpired = false;
				  foreach (var r in irecs.Where(
					  r => r.Expires <= Context.Time ||
					  r.RetryLimit > 0 && r.SendCount >= r.RetryLimit
					  ))
				  {
					  r.Status = Models.SendStatus.Failed;
					  DataContext.Update(r);
					  hasExpired = true;
				  }
				  if (hasExpired)
					  await DataContext.SaveChangesAsync();
				  return irecs.Where(r => r.Status == Models.SendStatus.Sending);
			  });

			string LastError = null;
			foreach (var SendRecord in recs.Where(r=>r.SendTime<=Context.Time))
			{
				try
				{
					var provider = NotificationSendProviderResolver(SendRecord.ProviderId);
					SendRecord.SendCount++;
					SendRecord.LastSendTime = Context.Time;
					Logger.Info($"[{TimeService.Now}]发送通知:{Context.Time}Id:{SendRecord.Id} 提供者:{SendRecord.ProviderId} BizIdent:{SendRecord.BizIdent} Targets:{SendRecord.Target} {SendRecord.Title} 第{SendRecord.SendCount}次");

					var result = await provider.Send(SendRecord);

					SendRecord.Result = result.Limit(1000);
					SendRecord.Status = Models.SendStatus.Success;
				}
				catch (Exception ex)
				{
					Logger.Warn(ex,$"[{TimeService.Now}]发送通知失败:{Context.Time} Id:{SendRecord.Id} 提供者:{SendRecord.ProviderId} BizIdent:{SendRecord.BizIdent} Targets:{SendRecord.Target} {SendRecord.Title} 第{SendRecord.SendCount}次");
					SendRecord.Error = ex.Message.Limit(1000);

					var nextTime = Context.Time.AddSeconds(SendRecord.RetryInterval);
					if (ex is ExternalServiceException ese && ese.Retryable &&
						nextTime < SendRecord.Expires && 
						(SendRecord.RetryLimit == 0 || SendRecord.SendCount < SendRecord.RetryLimit)
						)
					{
						SendRecord.SendTime = nextTime;
						LastError = ex.Message;
					}
					else
						SendRecord.Status = Models.SendStatus.Failed;
				}
			}

			await DataScope.Use("执行通知", DataContext =>
			{
				foreach (var SendRecord in recs.Where(r => r.SendTime <= Context.Time))
				{
					DataContext.Update(SendRecord);
				}
				return DataContext.SaveChangesAsync();
			});

			var nextRemindTime=recs
				.Where(r => r.Status == Models.SendStatus.Sending)
				.Select(r => r.SendTime)
				.DefaultIfEmpty()
				.Min();
			if (nextRemindTime != DateTime.MinValue)
			{
				Context.Message = LastError;
				Context.NextRemindTime = nextRemindTime;
			}
		}
	}
}
