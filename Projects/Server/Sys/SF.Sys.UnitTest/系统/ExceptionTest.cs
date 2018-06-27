using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SF.Sys;
using SF.Sys.Logging;
using SF.Sys.UnitTest;
using System.Threading.Tasks;
using SF.Sys.Services;
using System;

namespace SF.UT.ÏµÍ³
{
	[TestClass]
	public class ExceptionTest : TestBase
	{
		
		[TestMethod]
		public async Task Ç¶Ì×Òì³£()
		{
			await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
		   {
			   try
			   {
				   try
				   {
					   try
					   {
						   await Task.Delay(1);
						   throw new ArgumentException();

					   }
					   catch (Exception ex)
					   {
						   await Task.Delay(1);
						   Console.WriteLine("1");
						   throw;
					   }
					   finally
					   {
						   await Task.Delay(1);
						   Console.WriteLine("2");
					   }
				   }
				   catch (Exception ex)
				   {
					   await Task.Delay(1);
					   Console.WriteLine("3");
					   throw;
				   }
				   finally
				   {
					   await Task.Delay(1);
					   Console.WriteLine("4");
						   throw new ArgumentException();
				   }
			   }
			   catch (Exception ex)
			   {
				   await Task.Delay(1);
				   Console.WriteLine("5");
				   throw;
			   }
			   finally
			   {
				   await Task.Delay(1);
				   Console.WriteLine("6");
			   }
		   });
		}
		
	}


}
