using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SF.Sys;
using SF.Sys.Logging;
using SF.Sys.UnitTest;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.Settings;
using SF.Sys.Comments;
using System;

namespace SF.UT.系统
{
	[TestClass]
	public class CommentTest : TestBase
	{
		
		[TestMethod]
		public void GenericTypeCommment()
		{
			var ss = typeof(ISettingService<SystemSetting>);
			var re=ss.Comment();
			Assert.AreEqual("系统设置设置服务",re.Title);
		}
		
	}


}
