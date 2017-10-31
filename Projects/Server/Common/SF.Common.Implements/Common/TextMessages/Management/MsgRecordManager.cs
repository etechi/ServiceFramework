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
		QuerableEntitySource<ObjectKey<long>,MsgRecord,MsgRecordQueryArgument,DataModels.TextMessageRecord>,
		IMsgRecordManager,
		ITextMessageLogger
	{
		
		public EntityMsgRecordManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}

		protected override PagingQueryBuilder<DataModels.TextMessageRecord> PagingQueryBuilder =>
			PagingQueryBuilder<DataModels.TextMessageRecord>.Simple("time", b => b.Time, true);

	
		protected override IContextQueryable<MsgRecord> OnMapModelToDetail(IContextQueryable<DataModels.TextMessageRecord> Query)
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
