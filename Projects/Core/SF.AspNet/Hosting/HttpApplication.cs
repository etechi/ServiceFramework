using SF.Core.ServiceManagement;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SF.AspNet
{
	public class HttpApplication : System.Web.HttpApplication
	{
		static IAppInstance _AppInstance;
		public static IAppInstance AppInstance
		{
			get
			{
				if (_AppInstance == null)
					throw new InvalidOperationException();
				return _AppInstance;
			}
		}

		public static IServiceProvider ServiceProvider => AppInstance.ServiceProvider;
		static object InitLock = new object();
		public void SetAppInstance(IAppInstance Instance)
		{
			if (_AppInstance != null)
				throw new InvalidOperationException();
			_AppInstance = Instance;

		}
		public void StartAppInstance(Func<IAppInstance> Instance, HttpConfiguration cfg)
		{
			lock (InitLock)
			{
				if (_AppInstance == null)
				{
					SetAppInstance(Instance());
					//初始化MVC参数提供者
					ServiceProvider.InitMvcValueProvider();
				}
			}

			//初始化WebApi格式化器
			ServiceProvider.InitWebApiFormatter(cfg);

			//初始化WebApi,Mvc依赖注入
			ServiceProvider.ReplaceDependenceResolver(cfg);
		}
		public void StopAppInstance()
		{
			Disposable.Release(ref _AppInstance);
		}
	}
}
