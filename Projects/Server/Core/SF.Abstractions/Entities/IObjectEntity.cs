using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Entities
{
	public interface IObjectEntity :
		IEntityWithName,
		IEntityWithLogicState
	{

		DateTime CreatedTime { get; set; }

		DateTime UpdatedTime { get; set; }
	}
}
