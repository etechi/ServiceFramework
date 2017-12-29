
using SF.Sys.Entities;
using SF.Sys.AtLeastOnceActions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Data;
using SF.Sys.Services;
namespace SF.Sys.AtLeastOnceActions
{
	class AtLeastOnceActionManager :
		AutoModifiableEntityManager<
			ObjectKey<long>,
			AtLeastOnceAction,
			AtLeastOnceAction,
			AtLeastOnceActionQueryArgument,
			AtLeastOnceAction,
			DataModels.AtLeastOnceAction
			>,
		IAtLeastOnceActionManager
	{
		public AtLeastOnceActionManager(
			IEntityServiceContext ServiceContext
			) : base(ServiceContext)
		{
		}

		
	}
}
