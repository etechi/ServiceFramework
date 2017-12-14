using System;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.Sys.Hosting;
using SF.Sys.Logging;

namespace SF.Sys.UnitTest
{
	public static class EmptyAppBuilder
	{
		public static IAppInstanceBuilder Instance { get; } = new Lazy<IAppInstanceBuilder>(() =>
		{
			var ls = new LogService(new SF.Sys.Logging.MicrosoftExtensions.MSLogMessageFactory());
			ls.AddDebug();
			var b = new AppInstanceBuilder("Test", EnvironmentType.Production, new ServiceCollection(), ls);
			return b;
		}).Value;
	}
}
