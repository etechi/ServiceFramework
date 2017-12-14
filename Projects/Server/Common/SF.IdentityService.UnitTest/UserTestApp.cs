using SF.Auth.IdentityServices;
using System;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.Sys.Hosting;
using SF.Sys.UnitTest;
using SF.Sys.Data;

namespace SF.IdentityService.UnitTest
{
	public static class UserTestAppBuilder
	{
		public static IAppInstanceBuilder Instance { get; } =
			EmptyAppBuilder.Instance.With(sc =>
			{

				var ds = new DataSourceConfig
				{
					ConnectionString =
						"Data Source=.\\sqlexpress;User ID=sa;pwd=system;initial catalog=sfut;MultipleActiveResultSets=True;App=EntityFramework"
				};
				sc.AddEFCoreDataEntity();
				sc.AddDataContext(sp =>ds);

				sc.AddIdentityService();
				sc.AddIdentityServer4Support();
			});
	}
}
