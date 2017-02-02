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
	public class ServiceConfiguration : SF.Core.DI.ServiceConfiguration<ServiceConfiguration>
	{
		public ServiceConfiguration() : base(
			DIServiceCollection.Create(), 
			EnvironmentType.Development
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
			ServiceProvider.InitWebApiFormatter(cfg);

			//初始化MVC参数提供者
			ServiceProvider.InitMvcValueProvider();

			//初始化WebApi,Mvc依赖注入
			ServiceProvider.ReplaceDependenceResolver(cfg);
		}
		public static void RegisterDIHttpModule()
		{
			DIHttpModule.Register();
		}
		protected override void OnConfigServices(IDIServiceCollection Services, EnvironmentType EnvironmentType)
		{
			Services.UseNewtonsoftJson();
			Services.UseDataContext();
			Services.RegisterMvcControllers(GetType().Assembly);
			Services.RegisterWebApiControllers(GlobalConfiguration.Configuration);
			Services.UseNetworkService();

			Services.AddTransient<ICalc, Calc>();

			Services.UseWebApiNetworkService(GlobalConfiguration.Configuration);
		}
	}
}