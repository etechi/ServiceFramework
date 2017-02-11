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

[assembly: PreApplicationStartMethod(typeof(ServiceConfiguration), nameof(ServiceConfiguration.RegisterDIHttpModule))]

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

	public class ServiceConfiguration : SF.Core.DI.ServiceConfiguration<ServiceConfiguration>
	{
		public ServiceConfiguration() : this(EnvironmentTypeDetector.Detect())
		{

		}
		public ServiceConfiguration(EnvironmentType EnvironmentType) : base(
			DIServiceCollection.Create(),
			EnvironmentType
			)
		{
		}
		protected override IServiceProvider OnBuildServiceProvider(IDIServiceCollection Services, EnvironmentType EnvironmentType)
		{
			var sp= Services.BuildServiceProvider();
			return sp;
		}
		public static void Configure()
		{
			var cfg = GlobalConfiguration.Configuration;

			//初始化WebApi格式化器
			DefaultServiceProvider.InitWebApiFormatter(cfg);

			//初始化MVC参数提供者
			DefaultServiceProvider.InitMvcValueProvider();

			//初始化WebApi,Mvc依赖注入
			DefaultServiceProvider.ReplaceDependenceResolver(cfg);
		}
		public static void RegisterDIHttpModule()
		{
			DIHttpModule.Register();
		}
		protected override void OnConfigServices(IDIServiceCollection Services, EnvironmentType EnvType)
		{
			Services.UseNewtonsoftJson();
			Services.UseLocalFileCache();
			Services.UseSystemMemoryCache();
			Services.UseSystemTimeService();

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