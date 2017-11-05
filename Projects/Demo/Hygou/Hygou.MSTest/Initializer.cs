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
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.ServiceFeatures;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hygou.UT
{
	[TestClass]
	public class EnvInitializer : TestBase
	{
		[TestMethod]
		public async Task InitServices()
		{
			await Scope(async (IServiceProvider sp) =>
				{
					await sp.InitServices("service");
					return 0;
				}
				);
		}
		[TestMethod]
		public async Task InitProducts()
		{
			await Scope(async (IServiceProvider sp) =>
			{
				await sp.InitServices("product");
				return 0;
			}
				);
		}
		[TestMethod]
		public async Task InitData()
		{
			await Scope(async (IServiceProvider sp) =>
			{
				await sp.InitServices("data");
				return 0;
			}
				);
		}
	}
	

}
