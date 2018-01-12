
using SF.Sys.Entities;
using SF.Sys.Reminders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Data;
namespace SF.Sys.Reminders
{
	public class RemindRecordManager :
		AutoModifiableEntityManager<
			ObjectKey<long>,
			RemindRecord,
			RemindRecord,
			RemindRecordQueryArgument,
			RemindRecord,
			DataModels.DataRemindRecord
			>,
		IRemindRecordManager
	{
		public RemindRecordManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
	}

}
