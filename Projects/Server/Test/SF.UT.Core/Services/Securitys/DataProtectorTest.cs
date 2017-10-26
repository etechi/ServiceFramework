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
using SF.Core.ServiceFeatures;
using System.Threading.Tasks;
using SF.Auth.Users;
using SF.Entities;

namespace SF.UT.Services.Securitys
{
	public class DataProtectorTest : TestBase
	{
		[Fact]
		public async Task Create()
		{
			await Scope(async (IServiceProvider sp) =>
				await sp.TestService(
					sim => sim.NewDataProtectorService(),
					async (svc) =>
					{
						var data = Bytes.Random(10);
						var password = Bytes.Random(10);
						var dataEncrypted = await svc.Encrypt("test", data, DateTime.Now.AddDays(1), password);
						var dataDecrypted = await svc.Decrypt("test", dataEncrypted, DateTime.Now, (buf, len) => Task.FromResult(password));
						Assert.True(dataDecrypted.Compare(data) == 0);
						return 0;
					})
			);

		}
	}
	

}
