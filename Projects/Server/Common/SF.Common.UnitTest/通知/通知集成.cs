
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Sys.UnitTest;
using SF.Sys.Hosting;

namespace SF.Common.UT.通知
{
    public class 通知集成 : TestBase
	{
		public 通知集成() : base(UnitTest.TestAppBuilder.Instance) { }
		public 通知集成(IAppInstanceBuilder builder) : base(builder) { }



	}
}
