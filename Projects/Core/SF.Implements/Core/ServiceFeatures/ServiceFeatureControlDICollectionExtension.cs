using SF.Core.DI;
using System.Collections.Generic;
using System.Linq;
using System;
using SF.Metadata;
using System.Reflection;
using SF.Core.Serialization;
using SF.Core.NetworkService;
using SF.Core.ServiceFeatures;

namespace SF.Core.ServiceManagement
{
	public static class ServiceFeatureDICollectionExtension
	{
		public static void UseServiceFeatureControl(
					this IServiceCollection sc
					)
		{
			sc.AddScoped<IServiceFeatureControlService, ServiceFeatureControlService>();
			
		}
	}

}
