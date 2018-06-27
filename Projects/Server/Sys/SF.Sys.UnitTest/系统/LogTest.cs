using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SF.Sys;
using SF.Sys.Logging;
using SF.Sys.UnitTest;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.ServiceFeatures;
namespace SF.UT.ϵͳ
{
	[TestClass]
	public class LogTest : TestBase
	{
		
		[TestMethod]
		public async Task ��־���()
		{
			await NewServiceScope().InitServices().Use(sp =>
			{	
				var l = sp.Resolve<ILogger<LogTest>>();
				l.Info("test");
			});
		}
		
	}


}
