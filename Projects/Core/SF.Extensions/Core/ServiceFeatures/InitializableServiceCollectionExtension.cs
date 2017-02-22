using SF.Core.ServiceFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Core.DI
{
	public static class InitializableDIServiceCollectionExtension
	{
		class InitHelper : IServiceInitializable
		{
			public Func<Task> Callback { get; set; }
			public string Title { get; set; }
			public Task Init()
			{
				return Callback();
			}
		}
		public static IDIServiceCollection AddInitializer(
			this IDIServiceCollection sc, 
			string Title,
			Func<IServiceProvider,Task> Callback)
			{
				sc.Normal().AddSingleton<IServiceInitializable>(sp =>
					new InitHelper
					{
						Title=Title,
						Callback = () => Callback(sp)
					});
				return sc;
			}
		
		public static IDIServiceCollection AddInitializer(
			this IDIServiceCollection sc,
			string Title,
			Action<IServiceProvider> Callback)
		{
			sc.AddInitializer(
				Title,
				sp =>
				{
					Callback(sp);
					return Task.CompletedTask;
				}
				);
			return sc;
		}
		
	}
}
