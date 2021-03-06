﻿#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0


using SF.Sys.Entities.AutoTest;
using SF.Sys.Tests;
using System;
using System.Collections.Generic;
namespace SF.Sys.Entities.Tests.EntitySummaryValidators
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
			public EntitySummaryValidator(Action<TDetail, TSummary, string, List<TestResult>> FuncValidator) : base(FuncValidator)
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
