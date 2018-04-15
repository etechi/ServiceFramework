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
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.Sys.Services;
using SF.Sys.ServiceFeatures;
using SF.Sys.UnitTest;
using System.Collections.Generic;
using SF.Sys.Tests;

namespace SFShop.UT
{
	[TestClass]
	public class EnvInitializer : TestBase
	{
		[TestMethod]
		public async Task InitServices()
		{
			await NewServiceScope().Use(async (sp, ct) =>
			{
				await sp.InitServices("service");
			}
				);
		}
		[TestMethod]
		public async Task 导入_产品()
		{
			await NewServiceScope().Use(async (sp, ct) =>
			{
				await sp.InitServices("product");
			}
			);
		}
		[TestMethod]
		public async Task InitData()
		{
			await NewServiceScope().Use(async (sp, ct) =>
			{
				await sp.InitServices(
					"data",
					new Dictionary<string, string> {
						{ "host", "localhost:5000" },
						{ "ext-ident-postfix", "00" },
					});
			}
			);
		}

		[TestMethod]
		public async Task AutoEntityTest()
		{
			await NewServiceScope().Use(async (sp, ct) =>
			{
				var tcp = sp.Resolve<ITestCaseProvider>();
				var cases = tcp.GetTestCases();
				foreach (var c in cases)
					await tcp.Execute(c);
			});
		}
		[TestMethod]
		public async Task InitProducts()
		{
			await NewServiceScope().Use(async (sp, ct) =>
			{
				await sp.InitServices("product");
			}
				);
		}
	}
	

}
