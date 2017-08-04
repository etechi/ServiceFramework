using SF.Core.DI;
using System.Linq;
using SF.Metadata;
using System;
using SF.Core.ServiceManagement.Internals;
using SF.Core.ServiceManagement.Storages;
using SF.Core.ServiceManagement;
namespace SF.Core.Hosting
{
	public static class AppInstanceBuilderExtension
	{
		public static IAppInstance Build(this AppInstanceBuilder Builder)
		{
			return Builder.Build(sc => sc.BuildServiceResolver());
		}
	}

}
