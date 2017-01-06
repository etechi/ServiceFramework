using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using Newtonsoft.Json;
using SF.ServiceManagement;
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
		public IOperator op { get; set; }
		public int add { get; set; }
	}
	public class Agg : IAgg
	{
		IOperator op { get; }
		AggConfig cfg { get; }
		public Agg(IOperator op,AggConfig cfg)
		{
			this.op = op;
			this.cfg = cfg;
		}
		public int Sum(int[] ss) => 
			ss.Aggregate(cfg.add, (s, i) => op.Eval(s, cfg.op.Eval(i,i)));
	}

	

	public class ServiceManagerTest
    {
        [Fact]
        public void Test1()
        {
			var msc = new ManagedServiceCollection();
			msc.AddScoped<IOperator, Add>();
			msc.AddScoped<IOperator, Substract>();
			msc.AddScoped<IAgg, Agg>();

			var sc = new ServiceCollection();

			sc.AddScoped<IServiceConfigLoader, MemoryServiceSource>();
			sc.AddScoped<IDefaultServiceLocator>(sp=>(IDefaultServiceLocator)sp.GetRequiredService< IServiceConfigLoader>());

			sc.GetDIServiceCollection().UseManagedService(msc);
			
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
