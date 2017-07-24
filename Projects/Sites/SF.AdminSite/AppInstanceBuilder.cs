﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SF.Core.ServiceManagement;
using SF.AspNet.DI;
using System.Web.Http;
using SF.AdminSite;
using SF.Metadata;
using SF.Services.Test;
using SF.Core.TaskServices;
using SF.Core.Hosting;
using SF.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Data.Entity.Migrations;
using SF.AdminSite.Migrations;
using System.Data.Entity.Infrastructure;

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

	public class AppInstanceBuilder : SF.Core.Hosting.BaseAppInstanceBuilder<AppInstanceBuilder>
	{
		public static IAppInstance Build() =>
			Build(EnvironmentTypeDetector.Detect());
		 
		protected override ILogService OnCreateLogService()
		{
			var ls = new LogService(new Core.Logging.MicrosoftExtensions.MSLogMessageFactory());
			ls.AddAspNetTrace();

			ls.AsMSLoggerFactory()
				.AddDebug();
			return ls;
		}
		protected override IServiceCollection OnBuildServiceCollection()
			=> new SF.Core.ServiceManagement.ServiceCollection();
		protected override IServiceProvider OnBuildServiceProvider(IServiceCollection Services)
			=>Services.BuildServiceResolver();

		public static AppInstanceBuilder Default { get; } = new AppInstanceBuilder();

		protected override void OnInitStorage(IServiceProvider ServiceProvider)
		{
			if (EnvType!=EnvironmentType.Utils)
			{
				var configuration = new Configuration();
				var migrator = new DbMigrator(configuration);
				migrator.Update();
			}

			base.OnInitStorage(ServiceProvider);
		}
		protected override void OnConfigServices(IServiceCollection Services)
		{
			Services.AddLogService(LogService);

			Services.UseNewtonsoftJson();
			Services.UseSystemMemoryCache();
			Services.UseSystemTimeService();
			Services.UseTaskServiceManager();
			Services.UseDefaultMimeResolver();
			Services.UseSystemDrawing();
			Services.AddTransient<AppContext>(tsp => new AppContext(tsp));
			Services.UseEF6DataEntity<AppContext>();

			Services.UseDataContext();
			Services.UseDataEntity();
			Services.UseServiceFeatureControl();

			Services.AddTransient<ICalc, Calc>();

			var msc = Services.UseManagedService();
			msc.UseFilePathResolver();
			msc.UseLocalFileCache();
			msc.UseMediaService(EnvType);
			
			msc.AddScoped<IOperator, Add>();
			msc.AddScoped<IOperator, Substract>();
			msc.AddScoped<IAgg, Agg>();

			//msc.AddScoped<ICalc, Calc>();
			//msc.AddScoped<IOperator, Substract>();
			//msc.AddScoped<IAgg, Agg>();
			msc.AddScoped<ITestService, TestService>();
			msc.UseManagedServiceAdminServices();
			msc.UseIdentGenerator();


			Services.UseAspNetFilePathStructure();
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