using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	public interface IObjectEntity
	{
		string Name { get; set; }

		LogicObjectState ObjectState { get; set; }

		DateTime CreatedTime { get; set; }

		DateTime UpdatedTime { get; set; }
	}
}
