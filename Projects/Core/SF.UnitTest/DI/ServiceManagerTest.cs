using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using Newtonsoft.Json;
using SF.Services.Management;
using SF.DI;
using SF.DI.Microsoft;

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
		

			var sc = new ServiceCollection();

			sc.AddScoped<IServiceConfigLoader, MemoryServiceSource>();
			sc.AddScoped<IDefaultServiceLocator>(sp=>(IDefaultServiceLocator)sp.GetRequiredService< IServiceConfigLoader>());

			var isc = sc.GetDIServiceCollection();

			var msc = new ManagedServiceCollection(isc);
			msc.AddScoped<IOperator, Add>();
			msc.AddScoped<IOperator, Substract>();
			msc.AddScoped<IAgg, Agg>();

			var rsp = sc.BuildServiceProvider();
			var sf = rsp.GetRequiredService<IServiceScopeFactory>();
			using (var s = sf.CreateScope())
			{
				var sp = s.ServiceProvider;

				var ts = (MemoryServiceSource)sp.GetRequiredService<IServiceConfigLoader>();
				ts.SetConfig<IOperator, Add>("add", null);
				ts.SetConfig<IOperator, Substract>("substract", null);
				ts.SetConfig<IAgg, Agg>("agg1", new { op = "add", cfg = new { op = "add", add = 10000 } });
				ts.SetConfig<IAgg, Agg>("agg2", new { op = "add", cfg = new { op = "add", add = 20000 } });
				ts.SetDefaultService<IAgg>("agg1");

				var agg = sp.GetService<IAgg>();
				var re = agg.Sum(new[] { 1, 2, 3, 4 });
				Assert.Equal(10020, re);
			}
			using (var s = sf.CreateScope())
			{
				var sp = s.ServiceProvider;

				var ts = (MemoryServiceSource)sp.GetRequiredService<IServiceConfigLoader>();
				ts.SetDefaultService<IAgg>("agg2");

				var agg = sp.GetService<IAgg>();
				var re = agg.Sum(new[] { 1, 2, 3, 4 });
				Assert.Equal(20020, re);
			}
			using (var s = sf.CreateScope())
			{
				var sp = s.ServiceProvider;

				var ts = (MemoryServiceSource)sp.GetRequiredService<IServiceConfigLoader>();
				ts.SetConfig<IAgg, Agg>("agg2", new { op = "add", cfg = new { op = "substract", add = 20000 } });

				var agg = sp.GetService<IAgg>();
				var re = agg.Sum(new[] { 1, 2, 3, 4 });
				Assert.Equal(20000, re);
			}
		}
    }
}
