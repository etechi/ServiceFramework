using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Entities
{
	public interface IEntityWithScope
	{
		long? ScopeId { get; }
	}
}
