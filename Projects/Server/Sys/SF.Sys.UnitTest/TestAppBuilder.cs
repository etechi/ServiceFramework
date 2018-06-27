using System;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.Sys.Hosting;
using SF.Sys.Logging;
using SF.Sys.NetworkService;
using SF.Sys.BackEndConsole;

namespace SF.Sys.UnitTest
{
	public static class TestAppBuilder
	{
		public static IAppInstanceBuilder Instance { get; } = 
			new AppInstanceBuilder(null, EnvironmentType.Development).
				With(sc =>
				{
					var rootPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\");
					var binPath = System.IO.Path.Combine(rootPath, "bin\\Debug\\netcoreapp2.0\\");
					sc.AddTestFilePathStructure(
						binPath,
						rootPath
						);
					sc.AddMSConfiguration();
					sc.AddSystemServices(EnvironmentType.Development);
					sc.AddInMemoryEFCoreDbContext<TestDbContext>("test");
					sc.AddSingleton(new Moq.Mock<IInvokeContext>().Object);
					sc.AddSingleton(new Moq.Mock<IAccessTokenGenerator>().Object);
					sc.AddSingleton(new Moq.Mock<IAccessTokenValidator>().Object);
					sc.AddSingleton(new Moq.Mock<IUploadedFileCollection>().Object);
					sc.AddSingleton(new Moq.Mock<IBackEndConsoleBuilderCollection>().Object);
				});
	}
}
