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

namespace SF.Core.ServiceManagement
{
	public static class EntityTestDICollectionExtension
	{
		
		public static IServiceCollection AddEntityTestServices(this IServiceCollection sc)
		{
			sc.AddSingleton<IEntityTestHelperCache, EntityTestHelperCache>();
			sc.Add(typeof(IEntityTestContext<,,,,,>), typeof(EntityTestContext<,,,,,>), ServiceImplementLifetime.Scoped);

			sc.AddSingleton<IEntitySampleGeneratorProvider,DefaultEntitySampleGeneratorProvider>();
			sc.AddSingleton<IEntityCreateResultValidatorProvider, DefaultEntityCreateResultValidatorProvider>();
			sc.AddSingleton<IEntityUpdateResultValidatorProvider, DefaultEntityUpdateResultValidatorProvider>();
			sc.AddSingleton<IEntitySummaryValidatorProvider, DefaultEntitySummaryValidatorProvider>();
			sc.AddSingleton<IEntityDetailValidatorProvider, DefaultEntityDetailValidatorProvider>();


			return sc;
		}
	}
}
