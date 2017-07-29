using SF.Core.ServiceFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public static class InitializableDIServiceCollectionExtension
	{
		class InitHelper : IServiceInitializable
		{
			public Func<IServiceProvider,Task> Callback { get; set; }
			public string Title { get; set; }
			public int Priority { get; set; }
			public Task Init(IServiceProvider ServiceProvider)
			{
				return Callback(ServiceProvider);
			}
		}
		public static IServiceCollection AddInitializer(
			this IServiceCollection sc, 
			string Title,
			Func<IServiceProvider,Task> Callback,
			int Priority=0
			)
			{
				sc.AddSingleton<IServiceInitializable>(sp =>
					new InitHelper
					{
						Title=Title,
						Callback = isp => Callback(isp),
						Priority=Priority
					});
				return sc;
			}
		
		public static IServiceCollection AddInitializer(
			this IServiceCollection sc,
			string Title,
			Action<IServiceProvider> Callback,
			int Priority = 0)
		{
			sc.AddInitializer(
				Title,
				sp =>
				{
					Callback(sp);
					return Task.CompletedTask;
				},
				Priority
				);
			return sc;
		}
		
	}
}
