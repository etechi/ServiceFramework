using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using ServiceProtocol.Data.Entity;
namespace SF.Data.Services
{
	public enum DataActionType
	{
		Exists,
		Load,
		Query,

		Create,
		Update,
		Delete,

		BatchLoad,
		BatchUpdate,
		BatchDelete,
		
		Other
	}
	

	public interface IDataActionInvocation
	{
		DataActionType ActionType { get; }
		string ActionName { get; set; }
		object[] Arguments { get; set; }
		object Result { get; set; }
		Task Proceed();
		object OwnerId { get; set; }
		string ResourceType { get; }
		void AddPostAction(Action action, bool CallOnSaved = true);
	}
	
}
