using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using Xunit;
using SF.Applications;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.NetworkService;
using SF.Clients;

namespace Hygou.UT
{ 
	public static class TestApp
	{
		public static IAppInstanceBuilder Builder(EnvironmentType envType=EnvironmentType.Production)
		{
			return HygouApp.Setup(envType).
				With(sc =>
				{
					var rootPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\Hygou.Site\\");
					var binPath = System.IO.Path.Combine(rootPath, "bin\\Debug\\netcoreapp2.0\\");
					sc.AddTestFilePathStructure(
						binPath,
						rootPath
						);
					sc.AddSingleton(new Moq.Mock<IInvokeContext>().Object);
					sc.AddSingleton(new Moq.Mock<IUploadedFileCollection>().Object);
					sc.AddNetworkService();
					sc.AddLocalClientService();
				});
			
		}
	}
	

}
