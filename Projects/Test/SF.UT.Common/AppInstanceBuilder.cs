using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data.Storage;
using Xunit;
using SF.Applications;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.NetworkService;

namespace SF.UT
{
	public static class App
	{
		public static IAppInstanceBuilder Builder(EnvironmentType envType=EnvironmentType.Production)
		{
			return Applications.App.Builder(envType).
				With(sc =>
				{
					sc.UseConsoleDefaultFilePathStructure();
					sc.AddSingleton(new Moq.Mock<IInvokeContext>().Object);
					sc.AddSingleton(new Moq.Mock<IUploadedFileCollection>().Object);
				});
			
		}
	}
	

}
