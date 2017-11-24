#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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

namespace SFShop.UT
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
