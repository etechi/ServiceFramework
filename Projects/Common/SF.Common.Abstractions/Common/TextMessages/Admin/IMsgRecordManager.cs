using SF.Data;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common.TextMessages.Admin
{
	
	public interface IMsgRecordManager : 
		Data.Entity.IEntitySource<long,MsgRecord,MsgRecordQueryArgument>
	{
	}
}
