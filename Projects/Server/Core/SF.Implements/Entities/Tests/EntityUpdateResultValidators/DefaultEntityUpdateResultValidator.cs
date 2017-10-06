
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
namespace SF.Entities.Tests.EntityUpdateResultValidators
{
	class DefaultEntityUpdateResultValidatorProvider :
		EntityValidatorProvider,
		IEntityUpdateResultValidatorProvider
	{
		public DefaultEntityUpdateResultValidatorProvider(IValueTestHelperCache ValueTestHelperCache) : base(ValueTestHelperCache)
		{
		}

		class EntityUpdateResultValidator<TEditable> :
			EntitytValidator<TEditable, TEditable>,
			IEntityUpdateResultValidator<TEditable>
		{
			public EntityUpdateResultValidator(Action<TEditable, TEditable, List<TestResult>> FuncValidator) : base(FuncValidator)
			{
			}

			public TestResult ValidateUpdateResult(TEditable CreateArgument, TEditable UpdateResult)
				=> Validate(CreateArgument, UpdateResult);
		}


		public IEntityUpdateResultValidator<TEditable> GetUpdateResultValidator<TEditable>()
		{
			return new EntityUpdateResultValidator<TEditable>(
				GetValidator<TEditable, TEditable>()
				);
		}
	}
}
