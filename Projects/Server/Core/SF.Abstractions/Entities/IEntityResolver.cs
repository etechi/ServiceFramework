using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Core.ServiceManagement;

namespace SF.Entities
{
	public interface IEntityReference:
		IEntityWithId<string>,
		IEntityWithName
	{
		Task<object> Resolve();
	}
	public interface IEntityReferenceResolver
	{
        Task<IEntityReference[]> Resolve(long ServiceId, IEnumerable<string> Keys);
		Task<IEntityReference[]> Resolve(long? ScopeId, string Type, IEnumerable<string> Keys);
	}
}
