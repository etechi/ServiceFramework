
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
namespace SF.Entities.Tests.EntityDetailValidators
{
	class DefaultEntityDetailValidatorProvider :
		EntityValidatorProvider,
		IEntityDetailValidatorProvider
	{
		public DefaultEntityDetailValidatorProvider(IValueTestHelperCache ValueTestHelperCache) : base(ValueTestHelperCache)
		{
		}

		class EntityDetailValidator<TEditable, TDetail> :
			EntitytValidator<TEditable, TDetail>,
			IEntityDetailValidator<TEditable, TDetail>
		{
			public EntityDetailValidator(Action<TEditable, TDetail, List<AssertResult>> FuncValidator) : base(FuncValidator)
			{
			}

			public AssertResult ValidateDetail(TEditable LoadEditableResult, TDetail Detail)
				=> Validate(LoadEditableResult, Detail);
		}


		public IEntityDetailValidator<TEditable,TDetail> GetDetailValidator<TEditable, TDetail>()
		{
			return new EntityDetailValidator<TEditable,TDetail>(
				GetValidator<TEditable, TDetail>()
				);
		}
	}
}
