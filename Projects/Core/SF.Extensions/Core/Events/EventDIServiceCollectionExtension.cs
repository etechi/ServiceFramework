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
			public Func<IServiceProvider, T, Task<bool>> Validator { get; }
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
			Func<IServiceProvider, E, Task<bool>> Validator)
		{
			sc.AddScoped<IEventValidator<E>>(sp =>
				new EventValidator<E>(sp, Validator)
				);
			return sc;
		}

		public static IServiceCollection AddEventValidator<I0, I1, I2, I3, E>(
			this IServiceCollection sc,
			Func<I0, I1, I2, I3, E, Task<bool>> Validator)
			=> sc.AddScoped<IEventValidator<E>>(
				sp =>
					 new EventValidator<E>(
						 sp,
						(isp, e) =>
							isp.Invoke<I0, I1, I2, I3, Task<bool>>((i0,i1,i2,i3) => Validator(i0,i1,i2,i3, e))
					 )
				);
		public static IServiceCollection AddEventValidator<I0, I1, I2, E>(
			this IServiceCollection sc,
			Func<I0, I1, I2,  E, Task<bool>> Validator)
			=> sc.AddScoped<IEventValidator<E>>(
				sp =>
					 new EventValidator<E>(
						 sp,
						(isp, e) =>
							isp.Invoke<I0, I1, I2,  Task<bool>>((i0, i1, i2) => Validator(i0, i1, i2,  e))
					 )
				);
		public static IServiceCollection AddEventValidator<I0, I1,  E>(
			this IServiceCollection sc,
			Func<I0, I1,  E, Task<bool>> Validator)
			=> sc.AddScoped<IEventValidator<E>>(
				sp =>
					 new EventValidator<E>(
						 sp,
						(isp, e) =>
							isp.Invoke<I0, I1, Task<bool>>((i0, i1) => Validator(i0, i1, e))
					 )
				);
		public static IServiceCollection AddEventValidator<I0, E>(
			this IServiceCollection sc,
			Func<I0,  E, Task<bool>> Validator)
			=> sc.AddScoped<IEventValidator<E>>(
				sp =>
					 new EventValidator<E>(
						 sp,
						(isp, e) =>
							isp.Invoke<I0,  Task<bool>>((i0) => Validator(i0, e))
					 )
				);

	}
}
