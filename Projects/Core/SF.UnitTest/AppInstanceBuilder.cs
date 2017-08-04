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
	public class AppInstanceBuilder : SF.Applications.AppInstanceBuilder
	{
		protected override void OnConfigServices(IServiceCollection Services)
		{
			base.OnConfigServices(Services);
			Services.UseConsoleDefaultFilePathStructure();
			Services.AddSingleton(new Moq.Mock<IInvokeContext>().Object);
			Services.AddSingleton(new Moq.Mock<IUploadedFileCollection>().Object);
		}
	}
	

}
