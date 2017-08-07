using SF.Core.ServiceManagement;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SF.Core.ServiceFeatures;
using SF.Core.Logging;

namespace SF.Core.Hosting
{
	public static class IAppInstanceBuilderExtension
	{
		
		public static IAppInstanceBuilder With(this IAppInstanceBuilder builder, Func<IServiceProvider, IDisposable> action)
		{
			builder.AddStartupAction(ai => action(ai.ServiceProvider));
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
