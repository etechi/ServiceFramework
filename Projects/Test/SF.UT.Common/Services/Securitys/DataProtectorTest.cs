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
using SF.Core.ServiceFeatures;
using System.Threading.Tasks;
using SF.Auth.Identities;
using SF.Data;

namespace SF.UT.Services.Securitys
{
	public class DataProtectorTest : TestBase
	{
		[Fact]
		public async Task Create()
		{
			await ServiceProvider.TestService(
				sim => sim.NewDataProtectorService(),
				async svc =>
				{
					var data = Bytes.Random(10);
					var password = Bytes.Random(10);
					var dataEncrypted=await svc.Encrypt("test", data, DateTime.Now.AddDays(1), password);
					var dataDecrypted = await svc.Decrypt("test", dataEncrypted, DateTime.Now, (buf, len) => Task.FromResult(password));
					Assert.True(dataDecrypted.Compare(data) == 0);
				});

		}
	}
	

}
