
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
namespace SF.Entities.Tests.EntitySummaryValidators
{
	class DefaultEntitySummaryValidatorProvider :
		EntityValidatorProvider,
		IEntitySummaryValidatorProvider
	{
		public DefaultEntitySummaryValidatorProvider(IValueTestHelperCache ValueTestHelperCache) : base(ValueTestHelperCache)
		{
		}

		class EntitySummaryValidator<TDetail, TSummary> :
			EntitytValidator<TDetail, TSummary>,
			IEntitySummaryValidator<TDetail, TSummary>
		{
			public EntitySummaryValidator(Action<TDetail, TSummary, List<TestResult>> FuncValidator) : base(FuncValidator)
			{
			}

			public TestResult ValidateSummary(TDetail LoadEditableResult, TSummary Summary)
				=> Validate(LoadEditableResult, Summary);
		}


		public IEntitySummaryValidator<TDetail, TSummary> GetSummaryValidator<TDetail, TSummary>()
		{
			return new EntitySummaryValidator<TDetail, TSummary>(
				GetValidator<TDetail, TSummary>()
				);
		}
	}
}
