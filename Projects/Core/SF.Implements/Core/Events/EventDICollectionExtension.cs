using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Metadata;
using SF.Entities;
using SF.Core.Events;

namespace SF.Core.ServiceManagement
{
	public static class EventDICollectionExtension
	{
		public static IServiceCollection AddEventServices(this IServiceCollection sc)
		{
			var em = new EventManager();
			sc.AddSingleton<IEventEmitter>(r=>em);
			sc.AddSingleton<ISourceResolver>(r => em);
			sc.Add(typeof(IEventSubscriber<>), typeof(EventSubscriber<>), ServiceImplementLifetime.Scoped);
			return sc;
		}
	}
}
