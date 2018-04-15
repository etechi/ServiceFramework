using SF.Sys.Hosting;
using SF.Sys.Services;
using System;
using System.Threading.Tasks;
using SF.Sys.ServiceFeatures;

namespace SFShop.UT
{
	public class TestBase : SF.Sys.UnitTest.TestBase
	{

		public TestBase() : base(TestAppBuilder.Instance)
		{

		}
		protected override async Task OnInitServices(IServiceProvider sp)
		{
			await sp.BootServices();
			//await base.OnInitServices(sp);
		}

	}



}
