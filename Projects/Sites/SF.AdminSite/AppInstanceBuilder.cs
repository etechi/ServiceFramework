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
using SF.Management.MenuServices.Models;

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

			msc.UseMenuService(InitDefaultMenu);

			Services.UseAspNetFilePathStructure();
			if (EnvType != EnvironmentType.Utils)
			{

				Services.RegisterMvcControllers(GetType().Assembly);
				Services.RegisterWebApiControllers(GlobalConfiguration.Configuration);
				Services.UseNetworkService();
				Services.UseWebApiNetworkService(GlobalConfiguration.Configuration);
			}

		}

		static MenuItem[] InitDefaultMenu()
		{
			return new[]
			{
				new MenuItem
				{
					Name="会员管理",
					Children = new[]
					{
						new MenuItem
						{
							Name="会员",
							Action=MenuItemAction.Link,
							ActionArgument="http://www.sina.com.cn"
						},
						new MenuItem
						{
							Name="渠道管理",
							Action=MenuItemAction.Link,
							ActionArgument="http://www.sina.com.cn"
						},
						new MenuItem
						{
							Name="模拟会员",
							Action=MenuItemAction.Link,
							ActionArgument="http://www.sina.com.cn"
						}
					}
				},
				new MenuItem
				{
					Name="产品管理",
					Children=new[]{
						new MenuItem
						{
							Name="产品",
							Action=MenuItemAction.Link,
							ActionArgument="http://www.sina.com.cn"
						},
						new MenuItem
						{
							Name="产品类型",
							Action=MenuItemAction.Link,
							ActionArgument="http://www.sina.com.cn"
						},
						new MenuItem
						{
							Name="产品目录",
							Action=MenuItemAction.Link,
							ActionArgument="http://www.sina.com.cn"
						}
					}
                },
				new MenuItem
				{
					Name="交易管理",
					Children = new[]
					{
						new MenuItem
						{
							Name="订单管理",
							Children = new[]
							{
								new MenuItem
								{
									Name="交易",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="交易项目",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								}
							}
						},
						new MenuItem
						{
							Name="发货管理",
							Children = new[]
							{
								new MenuItem
								{
									Name="发货计划",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="发货地址",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="发货渠道",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="发货地址",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="快递查询",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								}
							}
						},
						new MenuItem
						{
							Name="自动发货管理",
							Children = new[]
							{
								new MenuItem
								{
									Name="自动发货规格",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="导入自动发货数据",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="自动发货导入记录查询",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="自动发货记录查询",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								}
							}
						}
					}
				},
				new MenuItem
				{
					Name="财务管理",
					Children=new[]{
						new MenuItem
						{
							Name="账户",
							Action=MenuItemAction.Link,
							ActionArgument="http://www.sina.com.cn"
						},
						new MenuItem
						{
							Name="充值记录",
							Action=MenuItemAction.Link,
							ActionArgument="http://www.sina.com.cn"
						},
						new MenuItem
						{
							Name="提现记录",
							Action=MenuItemAction.Link,
							ActionArgument="http://www.sina.com.cn"
						},
						new MenuItem
						{
							Name="退款记录",
							Action=MenuItemAction.Link,
							ActionArgument="http://www.sina.com.cn"
						},
						new MenuItem
						{
							Name="账户科目",
							Action=MenuItemAction.Link,
							ActionArgument="http://www.sina.com.cn"
						}
					}
				},
				new MenuItem
				{
					Name="促销管理",
					Children=new[]
					{
						new MenuItem
						{
							Name="活动管理",
							Children=new[]
							{
								new MenuItem{
									Name="活动计划",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem{
									Name="活动参与记录",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem{
									Name="活动获奖记录",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},

							}
						},
						new MenuItem
						{
							Name="专属管理",
							Children=new[]
							{
								new MenuItem{
									Name="专属活动记录",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem{
									Name="专属活动参与记录",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								}
							}
						},
						new MenuItem
						{
							Name="优惠券",
							Children=new[]
							{
								new MenuItem{
									Name="优惠券计划",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem{
									Name="优惠券发放记录",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem{
									Name="优惠券使用记录",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem{
									Name="优惠券模板",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								}
							}
						},
						new MenuItem
						{
							Name="实物奖品",
							Children=new[]
							{
								new MenuItem{
									Name="实物奖品发放计划",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem{
									Name="实物奖品发放记录",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								}
							}
						}

					}
				},
				new MenuItem
				{
					Name="客服管理",
					Children=new[]
					{
						new MenuItem
						{
							Name="用户反馈",
							Action=MenuItemAction.Link,
							ActionArgument="http://www.sina.com.cn"
						},
						new MenuItem
						{
							Name="公告通知",
							Children = new[]
							{
								new MenuItem
								{
									Name="全体通知",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="会员通知",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								}
							}
							
						}
					}

				},				
				new MenuItem
				{
					Name="内容管理",
					Children=new[]
					{
						new MenuItem
						{
							Name="PC页面管理",
							Children=new[]
							{
								new MenuItem
								{
									Name="PC页面头部菜单",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="PC页面头部产品分类",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="PC页面头部幻灯片",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="PC页面尾部菜单",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="PC广告区",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
							}
						},
						new MenuItem
						{
							Name="移动端管理",
							Children=new[]
							{
								new MenuItem
								{
									Name="移动端头部菜单",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="移动端产品分类",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="移动端幻灯片",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								}
							}
						},
						new MenuItem
						{
							Name="文档管理",
							Children=new[]
							{
								new MenuItem
								{
									Name="PC系统介绍",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="PC帮助",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="移动端系统介绍",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								},
								new MenuItem
								{
									Name="移动端帮助",
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn"
								}
							}
						},
					}
				},
				new MenuItem
				{
					Name="系统管理",
					Children=new[]
					{
						new MenuItem
						{
							Name="业务权限管理",
							Children=new []
							{
								new MenuItem
								{
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn",
									Name="业务管理员"
								},
								new MenuItem
								{
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn",
									Name="业务管理角色"
								},
								new MenuItem
								{
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn",
									Name="业务管理权限"
								},
								new MenuItem
								{
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn",
									Name="业务操作日志"
								},
							}
						},
						new MenuItem
						{
							Name="系统权限管理",
							Children=new []
							{
								new MenuItem
								{
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn",
									Name="系统管理员"
								},
								new MenuItem
								{
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn",
									Name="系统管理角色"
								},
								new MenuItem
								{
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn",
									Name="系统管理权限"
								},
								new MenuItem
								{
									Action=MenuItemAction.Link,
									ActionArgument="http://www.sina.com.cn",
									Name="系统操作日志"
								},
							}
						},
						new MenuItem
						{
							Name="系统服务管理",
							Children=new[]
							{
								new MenuItem
								{
									Action=MenuItemAction.EntityManager,
									ActionArgument="系统服务实例"
								},
								new MenuItem
								{
									Action=MenuItemAction.EntityManager,
									ActionArgument="系统服务定义"
								},
								new MenuItem
								{
									Action=MenuItemAction.EntityManager,
									ActionArgument="系统服务实现"
								}
							}
						}
					}
					
				}
			};
		}
	}
}