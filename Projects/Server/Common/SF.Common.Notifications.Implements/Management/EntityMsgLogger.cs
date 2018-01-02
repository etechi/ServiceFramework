using SF.Common.Notifications.Models;
using SF.Sys;
using SF.Sys.Data;
using SF.Sys.Linq;
using SF.Sys.Logging;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Common.Notifications.Management
{

	public class EntityMsgLogger : DefaultMsgLogger
	{
        public ITimeService TimeService { get; }
		public Sys.Services.IScoped<IDataContext> ScopedDataContext { get; }
		public IIdentGenerator IdentGenerator { get; }
		public EntityMsgLogger(
            ILogService LogService,
            Sys.Services.IScoped<IDataContext> ScopedDataContext,
            ITimeService TimeService,
			IIdentGenerator<DataModels.NotificationSendRecord> IdentGenerator
			) :base(LogService)
		{
            this.ScopedDataContext = ScopedDataContext;
            this.TimeService = TimeService;
			this.IdentGenerator = IdentGenerator;

		}
		class ActionLogger : IMsgActionLogger
		{
			public IMsgActionLogger BaseActionLogger { get; set; }
			public IIdentGenerator IdentGenerator { get; set; }
			public IDataSet<DataModels.NotificationSendRecord> ActionRecords { get; set; }
			public long MsgRecordId { get; set; }
			public int Count { get; set; }
			public int Completed { get; set; }
			public DateTime Time { get; set; }
			public string TrackEndityId { get; set; }
			public ITimeService TimeService { get; set; }
			public async Task<string> Add(NotificationSendArgument Arg, Func<Task<string>> Action)
			{
				Count++;
				var id = await IdentGenerator.GenerateAsync<DataModels.NotificationSendRecord>();
				var rec = new DataModels.NotificationSendRecord
				{
					Args = Json.Stringify(Arg.Arguments),
					Content = Arg.Content,
					Id = id,
					UserId = Arg.TargetId,
					NotificationId = MsgRecordId,
					ProviderId = Arg.MsgProviderId,
					Time = Time,
					BizIdent = TrackEndityId,
					Title = Arg.Title,
					Target = Arg.Target
				};
				await ActionRecords.Context.SaveChangesAsync();
				try
				{
					var re=await BaseActionLogger.Add(Arg,Action);
					rec.Result = re.Limit(1000);
					rec.EndTime = TimeService.Now;
					rec.Status = SendStatus.Success;
					ActionRecords.Add(rec);
					Completed++;
					return re;
				}
				catch(Exception e)
				{
					rec.Error = e.ToString().Limit(1000);
					rec.EndTime = TimeService.Now;
					rec.Status = SendStatus.Failed;
					ActionRecords.Add(rec);
					throw;
				}
			}
		}
		public override async Task<long> Add(long? targetUserId, Message message, Func<IMsgActionLogger, Task<long>> Action)
		{
			return await ScopedDataContext.Use(async ctx => {
				var id = await IdentGenerator.GenerateAsync<DataModels.Notification>();
				var msgRecords = ctx.Set<DataModels.Notification>();
				var rec=new DataModels.Notification
				{
					Id = id,
					UserId = targetUserId,
					Args = Json.Stringify(message),
					Time = TimeService.Now,
					TrackEntityId = message.BizTraceId,
					PolicyIdent = message.Policy,
					Status=SendStatus.Sending
				};
				var al = new ActionLogger
				{
					TimeService = TimeService,
					Time = rec.Time,
					MsgRecordId = id,
					IdentGenerator = IdentGenerator,
					TrackEndityId = rec.TrackEntityId,
					ActionRecords = ctx.Set<DataModels.NotificationSendRecord>()
				};
				try
				{
					rec.PolicyId=await base.Add(
						targetUserId,
						message,
						async baseActionLogger =>
						{
							al.BaseActionLogger = baseActionLogger;
							return await Action(al);
						});
					rec.Status = SendStatus.Success;
					rec.CompletedActionCount = al.Completed;
					rec.ActionCount = al.Count;
					rec.EndTime = TimeService.Now;
					msgRecords.Add(rec);
					await ctx.SaveChangesAsync();
				}
				catch(Exception e)
				{
					rec.Error = e.ToString().Limit(1000);
					rec.Status = SendStatus.Failed;
					rec.CompletedActionCount = al.Completed;
					rec.ActionCount = al.Count;
					rec.EndTime = TimeService.Now;
					msgRecords.Add(rec);
					await ctx.SaveChangesAsync();
					throw;
				}
				return id;
			});
		}

		//public override async Task PostSend(object Context, IEnumerable<MessageSendResult> results, Exception error)
		//{
		//          var (ictx,id) = (Tuple<object, long>)Context;
		//          await base.PostSend(ictx, results, error);
		//	await ScopedDataContext.Use(async ctx =>
		//	{
		//		var set=
		//		await set.Update(
		//			r => r.Id == id,
		//			r =>
		//			{
		//				r.Status = error == null ? SendStatus.Completed : SendStatus.Failed;
		//				r.Error = error == null ? null : error.ToString();
		//				r.TargetResults = Json.Stringify(results);
		//				r.CompletedTime = TimeService.Now;
		//				r.Targets = "ignored";
		//			}
		//		);
		//		return 0;
		//	}
		//	);
		//      }
		//public override async Task<object> PreSend(IServiceInstanceDescriptor Service, Message message, long? targetUserId, string[] targets)
		//{
		//          var re=await base.PreSend(Service, message, targetUserId, targets);

		//	return await ScopedDataContext.Use(async set =>
		//	{
		//		var r = new DataModels.MsgActionRecord
		//		{
		//			Body = message.Body,
		//			Title = message.Title,
		//			Sender = message.Sender,
		//			ServiceId = Service.InstanceId,
		//			Headers = message.Headers == null ? null : Json.Stringify(message.Headers),
		//			Args = message.Arguments == null ? null : Json.Stringify(message.Arguments),
		//			Targets = targets.Join("\n"),
		//			CreatedTime = TimeService.Now,
		//			Status = SendStatus.Sending,
		//			TrackEntityId = message.TrackEntityId,
		//			TargetUserId = targetUserId
		//		};
		//		set.Add(r);
		//		await set.Context.SaveChangesAsync();
		//		return Tuple.Create(re, r.Id);
		//	});
		//      }
	}

}
