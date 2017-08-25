using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Entities
{
	public interface IEventEntity
	{
		long? UserId { get; set; }
		DateTime Time { get; set; }
	}
}
