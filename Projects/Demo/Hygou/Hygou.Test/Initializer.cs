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

namespace Hygou.UT
{
	public class EnvInitializer : TestBase
	{
		[Fact]
		public async Task InitServices()
		{
			await Scope(async (IServiceProvider sp) =>
				{
					await sp.InitServices("service");
					return 0;
				}
				);
		}
		[Fact]
		public async Task InitProducts()
		{
			await Scope(async (IServiceProvider sp) =>
			{
				await sp.InitServices("product");
				return 0;
			}
				);
		}
		[Fact]
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
