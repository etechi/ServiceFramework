using System;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.Sys.Hosting;
using SF.Sys.Logging;
using SF.Sys.NetworkService;
using SF.Sys.BackEndConsole;
using SF.Sys;

namespace SF.Common.UnitTest
{
	public static class TestAppBuilder
	{
		public static IAppInstanceBuilder Instance { get; } =
			SF.Sys.UnitTest.TestAppBuilder.Instance.With(sc =>
			{
				sc.AddCommonServices(new CommonServicesSetting
				{
					EnvType = EnvironmentType.Development,
				});
			});
	}
}
