
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
using SF.ADT;

namespace SF.Entities.Tests.EntityDetailToSummaryConverters
{
	class DefaultEntityDetailToSummaryConverterProvider : 
		IEntityDetailToSummaryConverterProvider
	{
		public IEntityDetailToSummaryConverter<TDetail, TSummary> GetDetailToSummaryConverter<TDetail, TSummary>()
		{
			return new EntityDetailToSummaryConverter<TDetail, TSummary>();
		}

		class EntityDetailToSummaryConverter<TDetail, TSummary> :
			IEntityDetailToSummaryConverter<TDetail, TSummary>
		{
			public TSummary ConvertToSummary(TDetail Detail)
			{
				return Poco.Map<TDetail, TSummary>(Detail);
			}
		}

	}
}
