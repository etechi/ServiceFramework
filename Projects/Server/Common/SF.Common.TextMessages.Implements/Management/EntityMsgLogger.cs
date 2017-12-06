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

namespace SF.Common.TextMessages.Management
{

	public class EntityMsgLogger : DefaultMsgLogger
	{
        public ITimeService TimeService { get; }
		public Sys.Services.IScoped<IDataSet<DataModels.MsgActionRecord>> ScopedDataSet { get; }
		public IIdentGenerator IdentGenerator { get; }
		public EntityMsgLogger(
            ILogger<DefaultMsgLogger> Logger,
            Sys.Services.IScoped<IDataSet<DataModels.MsgActionRecord>> ScopedDataSet,
            ITimeService TimeService,
			IIdentGenerator<DataModels.MsgActionRecord> IdentGenerator
			) :base(Logger)
		{
            this.ScopedDataSet = ScopedDataSet;
            this.TimeService = TimeService;
			this.IdentGenerator = IdentGenerator;

		}

		public override async Task PostSend(object Context, IEnumerable<MessageSendResult> results, Exception error)
		{
            var (ictx,id) = (Tuple<object, long>)Context;
            await base.PostSend(ictx, results, error);
			await ScopedDataSet.Use(async set =>
			{
				await set.Update(
					r => r.Id == id,
					r =>
					{
						r.Status = error == null ? SendStatus.Completed : SendStatus.Failed;
						r.Error = error == null ? null : error.ToString();
						r.TargetResults = Json.Stringify(results);
						r.CompletedTime = TimeService.Now;
						r.Targets = "ignored";
					}
				);
				return 0;
			}
			);
        }
		public override async Task<object> PreSend(IServiceInstanceDescriptor Service, Message message, long? targetUserId, string[] targets)
		{
            var re=await base.PreSend(Service, message, targetUserId, targets);

			return await ScopedDataSet.Use(async set =>
			{
				var r = new DataModels.MsgActionRecord
				{
					Body = message.Body,
					Title = message.Title,
					Sender = message.Sender,
					ServiceId = Service.InstanceId,
					Headers = message.Headers == null ? null : Json.Stringify(message.Headers),
					Args = message.Arguments == null ? null : Json.Stringify(message.Arguments),
					Targets = targets.Join("\n"),
					CreatedTime = TimeService.Now,
					Status = SendStatus.Sending,
					TrackEntityId = message.TrackEntityId,
					TargetUserId = targetUserId
				};
				set.Add(r);
				await set.Context.SaveChangesAsync();
				return Tuple.Create(re, r.Id);
			});
        }
	}

}
