using SF.Core;
using SF.Core.ServiceManagement;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Common.TextMessages.Management
{
	public class EntityMsgRecordManager :
		QuerableEntitySource<long,MsgRecord,MsgRecordQueryArgument,DataModels.TextMessageRecord>,
		IMsgRecordManager,
		ITextMessageLogger
	{
		
		public EntityMsgRecordManager(
			IDataSetEntityManager<DataModels.TextMessageRecord> Manager
			) : base(Manager)
		{
		}

		protected override PagingQueryBuilder<DataModels.TextMessageRecord> PagingQueryBuilder =>
			PagingQueryBuilder<DataModels.TextMessageRecord>.Simple("time", b => b.Time, true);

	
		protected override IContextQueryable<MsgRecord> OnMapModelToInternal(IContextQueryable<DataModels.TextMessageRecord> Query)
		{
			return Query.SelectEventEntity(m => new MsgRecord
			{
				Id = m.Id,
				Args=m.Args,
				Body=m.Body,
				CompletedTime=m.CompletedTime,
				Error=m.Error,
				Headers=m.Headers,
				Sender=m.Sender,
				ServiceId=m.ServiceId,
				Status=m.Status,
				Target=m.Target,
				Title=m.Title,
				Result=m.Result,
				TrackEntityId=m.TrackEntityId
			});
		}
		protected override IContextQueryable<DataModels.TextMessageRecord> OnBuildQuery(IContextQueryable<DataModels.TextMessageRecord> Query, MsgRecordQueryArgument Arg, Paging paging)
		{
			var scopeid = ServiceInstanceDescriptor.ParentInstanceId;
			var q = Query.Where(m=>m.ScopeId==scopeid)
				.Filter(Arg.Id, r => r.Id)
				.Filter(Arg.ServiceId,r=>r.ServiceId)
				.Filter(Arg.Target,r=>r.Target)
				.Filter(Arg.TargeUserId, r => r.UserId)
				.Filter(Arg.Time, r => r.Time)
				;
			
			return q;
		}

		public async Task<long> BeginSend(long ServiceId, string Target, long? TargetUserId,Message message)
		{
			var re=DataSet.Add(new DataModels.TextMessageRecord
			{
				Id = await IdentGenerator.GenerateAsync("文本消息记录"),
				Args = Json.Stringify(message.Arguments),
				Body = message.Body,
				Headers = Json.Stringify(message.Headers),
				ScopeId = ServiceInstanceDescriptor.ParentInstanceId,
				Sender = message.Sender,
				ServiceId = ServiceId,
				Status = SendStatus.Sending,
				Target = Target,
				UserId = TargetUserId,
				Time = Now,
				TrackEntityId = message.TrackEntityId
			});
			await DataSet.Context.SaveChangesAsync();
			return re.Id;
		}

		public async Task EndSend(long MessageId, string ExtIdent, string Error)
		{
			var r = await DataSet.FindAsync(MessageId);
			if (r == null)
				return;
			r.Error = Error;
			r.Result = ExtIdent;
			r.CompletedTime = Now;
			r.Status = Error == null ? SendStatus.Completed : SendStatus.Failed;
			DataSet.Update(r);
			await DataSet.Context.SaveChangesAsync();
		}
	}

}
