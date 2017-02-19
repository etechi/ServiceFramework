using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SF.Core.DI;
using SF.Core.DI.MicrosoftExtensions;
using SF.AspNet.DI;
using System.Web.Http;
using SF.AdminSite;
using SF.Metadata;
using SF.Core.ManagedServices;
using SF.Services.Test;
using SF.Core.TaskServices;
using SF.Core.Hosting;


namespace SF.AdminSite
{
	[NetworkService]
	public interface ICalc
	{
		int Add(int a, int b);
	}
	public class Calc : ICalc
	{
		public int Add(int a, int b)
		{
			return a + b;
		}
	}

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
		[Comment("操作1")]
		public IOperator Op { get; set; }

		[Comment("增加")]
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

		public Agg(
			[Comment("操作")]
			IOperator op,
			[Comment("设置")]
			AggConfig cfg
			)
		{
			this.op = op;
			this.Cfg = cfg;
		}
		public int Sum(int[] ss) =>
			ss.Aggregate(Cfg.Add, (s, i) => GetOp().Eval(s, Cfg.Op.Eval(i, i)));
	}

	public class AppInstanceBuilder : SF.Core.Hosting.BaseAppInstanceBuilder
	{
		public AppInstanceBuilder() : this(EnvironmentTypeDetector.Detect())
		{

		}
		public AppInstanceBuilder(EnvironmentType EnvironmentType) : base(EnvironmentType)
		{
		}
		protected override IDIServiceCollection OnBuildServiceCollection()
			=> DIServiceCollection.Create();
		protected override IServiceProvider OnBuildServiceProvider(IDIServiceCollection Services)
			=>Services.BuildServiceProvider();

		public static AppInstanceBuilder Default { get; } = new AppInstanceBuilder();

		protected override void OnConfigServices(IDIServiceCollection Services)
		{

			Services.UseNewtonsoftJson();
			Services.UseLocalFileCache();
			Services.UseSystemMemoryCache();
			Services.UseSystemTimeService();
			Services.UseTaskServiceManager();

			Services.AddTransient<AppContext>(tsp => new AppContext(tsp));
			Services.UseEF6DataEntity<AppContext>();

			Services.UseDataContext();
			Services.UseDataEntity();

			Services.AddTransient<ICalc, Calc>();

			var msc = Services.UseManagedService();


			msc.AddScoped<IOperator, Add>();
			msc.AddScoped<IOperator, Substract>();
			msc.AddScoped<IAgg, Agg>();

			//msc.AddScoped<ICalc, Calc>();
			//msc.AddScoped<IOperator, Substract>();
			//msc.AddScoped<IAgg, Agg>();
			msc.AddScoped<ITestService, TestService>();

			msc.UseManagedServiceAdminServices();
			msc.UseIdentGenerator();


			if (EnvType != EnvironmentType.Utils)
			{
				Services.RegisterMvcControllers(GetType().Assembly);
				Services.RegisterWebApiControllers(GlobalConfiguration.Configuration);
				Services.UseNetworkService();
				Services.UseWebApiNetworkService(GlobalConfiguration.Configuration);
			}


		}
	}
}