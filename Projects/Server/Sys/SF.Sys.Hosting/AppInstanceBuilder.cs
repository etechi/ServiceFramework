﻿#region Apache License Version 2.0
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

using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Sys.Hosting
{
	public static class IAppInstanceBuilderExtension
	{
		class StartupAction : IAppStartupAction
		{
			Func<IServiceProvider, IDisposable> action { get; }
			public StartupAction(Func<IServiceProvider, IDisposable> action)
			{
				this.action = action;
			}
			public IDisposable Execute(IAppInstance App)
			{
				return action(App.ServiceProvider);
			}
		}

		public static IServiceCollection AddStartupAction(this IServiceCollection Services, Func<IServiceProvider, IDisposable> action)
		{
			return Services.AddSingleton((IAppStartupAction)new StartupAction(action));
		}

		public static IAppInstanceBuilder With(this IAppInstanceBuilder builder, Func<IServiceProvider, IDisposable> action)
		{
			builder.Services.AddStartupAction(action);
			return builder;
		}
		public static IAppInstanceBuilder OnEnvType(this IAppInstanceBuilder builder, Predicate<EnvironmentType> EnvType, Func<IServiceProvider, IDisposable> action)
			=> EnvType(builder.EnvType) ? builder.With(action) : builder;

		public static IAppInstanceBuilder OnEnvType(this IAppInstanceBuilder builder, EnvironmentType EnvType, Func<IServiceProvider, IDisposable> action)
			=> builder.OnEnvType(e=>e == EnvType,action) ;

		public static IAppInstanceBuilder OnUtils(this IAppInstanceBuilder builder, Func<IServiceProvider, IDisposable> action)
			=> builder.OnEnvType( EnvironmentType.Utils,action);

		public static IAppInstanceBuilder OnProduction(this IAppInstanceBuilder builder, Func<IServiceProvider, IDisposable> action)
			=> builder.OnEnvType(EnvironmentType.Production, action);

		public static IAppInstanceBuilder OnStaging(this IAppInstanceBuilder builder, Func<IServiceProvider, IDisposable> action)
			=> builder.OnEnvType(EnvironmentType.Staging, action);

		public static IAppInstanceBuilder OnDevelopment(this IAppInstanceBuilder builder, Func<IServiceProvider, IDisposable> action)
			=> builder.OnEnvType(EnvironmentType.Development, action);



		public static IAppInstanceBuilder With(this IAppInstanceBuilder builder, Action<IServiceCollection,EnvironmentType> action)
		{
			action(builder.Services, builder.EnvType);
			return builder;
		}
		public static IAppInstanceBuilder With(this IAppInstanceBuilder builder, Action<IServiceCollection> action)
			=> builder.With((sc, env) => action(sc));

		public static IAppInstanceBuilder OnEnvType(this IAppInstanceBuilder builder, Predicate<EnvironmentType> EnvType, Action<IServiceCollection, EnvironmentType> action)
			=> EnvType(builder.EnvType) ? builder.With(action) : builder;


		public static IAppInstanceBuilder OnEnvType(this IAppInstanceBuilder builder, Predicate<EnvironmentType> EnvType, Action<IServiceCollection> action)
			=> EnvType(builder.EnvType) ? builder.With(action) : builder;

		public static IAppInstanceBuilder OnEnvType(this IAppInstanceBuilder builder, EnvironmentType EnvType, Action<IServiceCollection> action)
			=> builder.OnEnvType(e => e == EnvType, action);

		public static IAppInstanceBuilder OnUtils(this IAppInstanceBuilder builder, Action<IServiceCollection> action)
			=> builder.OnEnvType(EnvironmentType.Utils, action);

		public static IAppInstanceBuilder OnProduction(this IAppInstanceBuilder builder, Action<IServiceCollection> action)
			=> builder.OnEnvType(EnvironmentType.Production, action);

		public static IAppInstanceBuilder OnStaging(this IAppInstanceBuilder builder, Action<IServiceCollection> action)
			=> builder.OnEnvType(EnvironmentType.Staging, action);

		public static IAppInstanceBuilder OnDevelopment(this IAppInstanceBuilder builder, Action<IServiceCollection> action)
			=> builder.OnEnvType(EnvironmentType.Development, action);

	}
}
