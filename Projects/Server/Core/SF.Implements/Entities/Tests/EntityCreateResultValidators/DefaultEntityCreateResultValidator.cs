
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
namespace SF.Entities.Tests.EntityCreateResultValidators
{
	class DefaultEntityCreateResultValidatorProvider : 
		EntityValidatorProvider,
		IEntityCreateResultValidatorProvider
	{
		public DefaultEntityCreateResultValidatorProvider(IValueTestHelperCache ValueTestHelperCache):base(ValueTestHelperCache)
		{
		}

		class EntityCreateResultValidator<TEditable> :
			EntitytValidator<TEditable,TEditable>,
			IEntityCreateResultValidator<TEditable>
		{
			public EntityCreateResultValidator(Action<TEditable, TEditable, List<AssertResult>> FuncValidator) : base(FuncValidator)
			{
			}

			public AssertResult ValidateCreateResult(TEditable CreateArgument, TEditable UpdateResult)
				=> Validate(CreateArgument, UpdateResult);
		}


		public IEntityCreateResultValidator<TEditable> GetCreateResultValidator<TEditable>()
		{
			return new EntityCreateResultValidator<TEditable>(
				GetValidator<TEditable, TEditable>()
				);
		}
	}
}
