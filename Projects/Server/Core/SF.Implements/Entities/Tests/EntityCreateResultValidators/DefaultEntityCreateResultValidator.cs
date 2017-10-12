#region Apache License Version 2.0
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
using SF.Metadata;

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
			public EntityCreateResultValidator(Action<TEditable, TEditable, string, List<TestResult>> FuncValidator) : base(FuncValidator)
			{
			}
			
			public TestResult ValidateCreateResult(TEditable CreateArgument, TEditable UpdateResult)
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
