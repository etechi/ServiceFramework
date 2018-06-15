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

using SF.Sys.Logging;
using SF.Sys.Services;
using System;
using System.Collections.Generic;

namespace SF.Sys.Hosting
{
	public class AppInstanceBuilder: IAppInstanceBuilder
	{
		public EnvironmentType EnvType { get; }
		public ILogService LogService { get;  }
		public string Name { get; }
		public IServiceCollection Services { get; }
		List<Func<IAppInstance, IDisposable>> StartupActions = new List<Func<IAppInstance, IDisposable>>();
		public void AddStartupAction(Func<IAppInstance, IDisposable> Action)
		{
			StartupActions.Add(Action);
		}
		public AppInstanceBuilder(
			string Name,
			EnvironmentType EnvType=EnvironmentType.Production, 
			IServiceCollection Services=null, 
			ILogService LogService=null
			)
		{
			if (LogService == null)
			{
				LogService = new LogService(new SF.Sys.Logging.MicrosoftExtensions.MSLogMessageFactory());
				//ls.AddDebug(SF.Sys.Logging.LogLevel.Debug );
				//ls.AddConsole(SF.Sys.Logging.LogLevel.Debug);
				LogService.AddNLog();
			}

			this.EnvType = EnvType;
			this.LogService = LogService;

			this.Services= Services ?? new SF.Sys.Services.ServiceCollection();
			this.Name = Name;
			this.Services.AddLogService(LogService);
		}
		public class AppInstance : IAppInstance
		{
			public EnvironmentType EnvType { get; }
			public string Name { get; }
			public IServiceProvider ServiceProvider { get; }
			internal IDisposable _Shutdown;
			public AppInstance(string Name, EnvironmentType EnvType, IServiceProvider ServiceProvider)
			{
				this.Name = Name;
				this.EnvType = EnvType;
				this.ServiceProvider = ServiceProvider;
			}
			public void Dispose()
			{
				var dsp = ServiceProvider as IDisposable;
				if (dsp != null)
					dsp.Dispose();
				Disposable.Release(ref _Shutdown);
			}
		}
		public IAppInstance Build(Func<IServiceCollection, IServiceProvider> BuildServiceProvider)
		{
			AppInstance ai = null;
			Services.AddTransient<IAppInstance>(isp => {
				if (ai == null)
					throw new InvalidOperationException();
				return ai;
			});
			var sp = BuildServiceProvider(Services);
			ai = new AppInstance(Name, EnvType, sp);

			var disposables = new List<IDisposable>();
			foreach(var act in StartupActions)
			{
				var d = act(ai);
				if(d!=null)
					disposables.Add(d);
			}
			ai._Shutdown = Disposable.Combine(disposables.ToArray());
			return ai;
		}
		
		
	}
}
