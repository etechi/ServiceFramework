using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Core.Events;
namespace SF.Core.ServiceManagement
{
	public static class EventDIServiceCollectionExtension
	{
		class EventValidator<T> : IEventValidator<T>
		{
			public IServiceProvider ServiceProvider { get; }
			public Func<IServiceProvider,T,Task<bool>> Validator { get; }
			public EventValidator(IServiceProvider ServiceProvider, Func<IServiceProvider, T, Task<bool>> Validator)
			{
				this.ServiceProvider = ServiceProvider;
				this.Validator = Validator;
			}
			public Task<bool> Validate(T Event)
			{
				return this.Validator(ServiceProvider, Event);
			}
		}
		public static IServiceCollection AddEventValidator<E>(
			this IServiceCollection sc,
			Func<IServiceProvider,E,Task<bool>> Validator)
		{
			sc.AddScoped<IEventValidator<E>>(sp =>
				new EventValidator<E>(sp, Validator)
				);
			return sc;
		}
	}
	public interface IEventValidator<T>
	{
		Task<bool> Validate(T Event);
	}

	public interface IEventSubscriber<T>
	{
		void Wait(Func<T,Task> Callback);
	}

	public interface IEventObservable
    {
        IDisposable Subscribe(Func<object,Task> observer);
    }
    public interface IEventSource
    {
		IEventObservable GetObservable(string Type);
    }

	public interface ISourceResolver
	{
		IEventSource GetSource(string Name);
	}

	public interface IEventEmitter
	{
		Task Emit(object Event,bool SyncMode=true);
	}
   
}
