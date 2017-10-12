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
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using SF.Core.ServiceManagement.Storages;
using SF.Core.ServiceManagement.Internals;
using SF.Core.ServiceManagement;

namespace SF.UT
{
	public interface IOperator
	{
		int Eval(int a, int b);
	}
	public class Add : IOperator
	{
		public int Eval(int a, int b) => a + b;
	}
	public class Substract : IOperator
	{
		public int Eval(int a, int b) => a - b;
	}

	public interface IAgg
	{
		int Sum(int[] ss);
	}
	public class AggConfig
	{
		public IOperator Op { get; set; }
		public int Add { get; set; }
	}
	public class Agg : IAgg
	{
		private readonly IOperator op;

		private IOperator GetOp()
		{
			return Op;
		}

		AggConfig Cfg { get; }

		public IOperator Op => Op1;

		public IOperator Op1 => op;

		public Agg(IOperator op,AggConfig cfg)
		{
			this.op = op;
			this.Cfg = cfg;
		}
		public int Sum(int[] ss) => 
			ss.Aggregate(Cfg.Add, (s, i) => GetOp().Eval(s, Cfg.Op.Eval(i,i)));
	}

	

	public class ServiceManagerTest
    {
        [Fact]
        public void Test1()
        {
			var isc = new ServiceCollection();


			//var isc = sc.GetDIServiceCollection();
			isc.AddManagedService();
			isc.UseMemoryManagedServiceSource();
			isc.AddMicrosoftMemoryCacheAsLocalCache();
			isc.AddNewtonsoftJson();
			var msc = isc.AddManagedService();
			msc.AddScoped<IOperator, Add>();
			msc.AddScoped<IOperator, Substract>();
			msc.AddScoped<IAgg, Agg>();

			var rsp = isc.BuildServiceResolver();
			var sf = rsp.Resolve<IServiceScopeFactory>();
			using (var s = sf.CreateServiceScope())
			{
				var sp = s.ServiceProvider;

				var ts = (MemoryServiceSource)sp.Resolve<IServiceConfigLoader>();
				ts.SetConfig<IOperator, Add>(1, null);
				ts.SetConfig<IOperator, Substract>(2, null);
				ts.SetConfig<IAgg, Agg>(3, new { op = 1, cfg = new { Op = 1, Add = 10000 } });
				ts.SetConfig<IAgg, Agg>(4, new { op = 1, cfg = new { Op = 1, Add = 20000 } });
				ts.SetDefaultService<IAgg>(3);

				var agg = sp.TryResolve<IAgg>();
				var re = agg.Sum(new[] { 1, 2, 3, 4 });
				Assert.Equal(10020, re);
			}
			using (var s = sf.CreateServiceScope())
			{
				var sp = s.ServiceProvider;

				var ts = (MemoryServiceSource)sp.Resolve<IServiceConfigLoader>();
				ts.SetDefaultService<IAgg>(4);

				var agg = sp.TryResolve<IAgg>();
				var re = agg.Sum(new[] { 1, 2, 3, 4 });
				Assert.Equal(20020, re);
			}
			using (var s = sf.CreateServiceScope())
			{
				var sp = s.ServiceProvider;

				var ts = (MemoryServiceSource)sp.Resolve<IServiceConfigLoader>();
				ts.SetConfig<IAgg, Agg>(4, new { op = 1, cfg = new { Op = 2, Add = 20000 } });

				var agg = sp.TryResolve<IAgg>();
				var re = agg.Sum(new[] { 1, 2, 3, 4 });
				Assert.Equal(20000, re);
			}
		}
    }
}
