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
using SF.Clients;

namespace SF.UT
{
	public static class TestApp
	{
		public static IAppInstanceBuilder Builder(EnvironmentType envType=EnvironmentType.Production)
		{
			return Net46App.Setup(envType).
				With(sc =>
				{
					sc.AddDataContext(new SF.Data.DataSourceConfig
					{
						ConnectionString = "data source=.\\SQLEXPRESS;initial catalog=sfadmin;user id=sa;pwd=system;MultipleActiveResultSets=True;App=EntityFramework"
					});

					sc.AddConsoleDefaultFilePathStructure();
					sc.AddSingleton(new Moq.Mock<IInvokeContext>().Object);
					sc.AddSingleton(new Moq.Mock<IUploadedFileCollection>().Object);

					sc.AddLocalClientService();
				});
			
		}
	}
	

}
