
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Core.Logging;
using SF.Core.Times;
using SF.Data;
using SF.Core.ServiceManagement;
using SF.Core.Events;
using SF.Services.Tests;
using System.Linq.Expressions;
using System.Reflection;
namespace SF.Entities.Tests.EntityQueryArgumentGenerators
{
	class DefaultEntityQueryArgumentGeneratorProvider:
		IEntityQueryArgumentGeneratorProvider
	{
		public IEntityQueryArgumentGenerator<TSummary, TQueryArgument> GetQueryArgumentGenerator<TSummary, TQueryArgument>()
		{
			throw new NotImplementedException();
		}

		class EntityQueryArgumentGenerator<TSummary, TQueryArgument> :
			IEntityQueryArgumentGenerator<TSummary, TQueryArgument>
		{
			public IEnumerable<QueryTestCase<TQueryArgument, TSummary>> GenerateQueryArgument(IEnumerable<TSummary> Summaries)
			{
				throw new NotImplementedException();
			}
		}


	}
}
