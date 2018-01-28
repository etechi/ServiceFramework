
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
	public class Remindable :
		IRemindable
	{
		public string Name => "通知发送";

		IDataScope DataScope { get; }
		TypedInstanceResolver<INotificationSendProvider> NotificationSendProviderResolver { get; }
		ITimeService TimeService { get; }
		
		public Remindable(
			ITimeService TimeService,
			IDataScope DataScope,
			TypedInstanceResolver<INotificationSendProvider> NotificationSendProviderResolver
			)
		{
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


			foreach (var SendRecord in recs.Where(r=>r.SendTime<=Context.Time))
			{
				try
				{
					var provider = NotificationSendProviderResolver(SendRecord.ProviderId);
					SendRecord.SendCount++;
					SendRecord.LastSendTime = Context.Time;

					var result = await provider.Send(SendRecord);

					SendRecord.Result = result.Limit(1000);
					SendRecord.Status = Models.SendStatus.Success;
				}
				catch (Exception ex)
				{
					SendRecord.Error = ex.Message.Limit(1000);
					var nextTime = Context.Time.AddSeconds(SendRecord.RetryInterval);
					if (nextTime < SendRecord.Expires && (SendRecord.RetryLimit == 0 || SendRecord.SendCount < SendRecord.RetryLimit))
						SendRecord.SendTime = nextTime;
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
				Context.NextRemindTime = nextRemindTime;
		}
	}
}
