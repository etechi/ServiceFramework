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
