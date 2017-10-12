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
using System.Reflection;
using SF.Metadata;
using SF.Entities;
using SF.Entities.Tests;
using SF.Entities.Tests.EntitySampleGenerators;
using SF.Entities.Tests.EntityCreateResultValidators;
using SF.Entities.Tests.EntityUpdateResultValidators;
using SF.Entities.Tests.EntitySummaryValidators;
using SF.Entities.Tests.EntityDetailValidators;
using SF.Entities.Tests.EntityQueryArgumentGenerators;
using SF.Entities.Tests.EntityDetailToSummaryConverters;
using SF.Entities.Tests.EntityTestors;
using SF.Services.Tests;

namespace SF.Core.ServiceManagement
{
	public static class EntityTestDICollectionExtension
	{
		
		public static IServiceCollection AddEntityTestServices(this IServiceCollection sc)
		{
			sc.AddSingleton<IEntityTestHelperCache, EntityTestHelperCache>();
			sc.Add(typeof(IEntityTestContext<,,,,,>), typeof(EntityTestContext<,,,,,>), ServiceImplementLifetime.Scoped);

			sc.AddSingleton<IEntitySampleGeneratorProvider,EntitySampleGeneratorProvider>();
			sc.AddSingleton<IEntityCreateResultValidatorProvider, DefaultEntityCreateResultValidatorProvider>();
			sc.AddSingleton<IEntityUpdateResultValidatorProvider, DefaultEntityUpdateResultValidatorProvider>();
			sc.AddSingleton<IEntitySummaryValidatorProvider, DefaultEntitySummaryValidatorProvider>();
			sc.AddSingleton<IEntityDetailValidatorProvider, DefaultEntityDetailValidatorProvider>();
			sc.AddSingleton<IEntityQueryArgumentGeneratorProvider, DefaultEntityQueryArgumentGeneratorProvider>();
			sc.AddSingleton<IEntityDetailToSummaryConverterProvider, DefaultEntityDetailToSummaryConverterProvider>();

			sc.AddTransient<ITestCaseProvider, DefaultEntityTestCaseProvider>();

			return sc;
		}
	}
}
