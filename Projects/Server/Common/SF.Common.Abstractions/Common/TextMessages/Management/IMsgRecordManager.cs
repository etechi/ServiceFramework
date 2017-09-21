using SF.Auth;
using SF.Data;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common.TextMessages.Management
{
	[EntityManager("文本消息记录")]
	[Authorize("admin")]
	[NetworkService]
	[Comment("文本消息记录")]
	public interface IMsgRecordManager : 
		Entities.IEntitySource<long,MsgRecord,MsgRecordQueryArgument>
	{
	}
}
