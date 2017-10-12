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
		//class EventValidator<T> : IEventValidator<T>
		//{
		//	public IServiceProvider ServiceProvider { get; }
		//	public Func<IServiceProvider, T, Task<bool>> Validator { get; }
		//	public EventValidator(IServiceProvider ServiceProvider, Func<IServiceProvider, T, Task<bool>> Validator)
		//	{
		//		this.ServiceProvider = ServiceProvider;
		//		this.Validator = Validator;
		//	}
		//	public Task<bool> Validate(T Event)
		//	{
		//		return this.Validator(ServiceProvider, Event);
		//	}
		//}
		//public static IServiceCollection AddEventValidator<E>(
		//	this IServiceCollection sc,
		//	Func<IServiceProvider, E, Task<bool>> Validator)
		//{
		//	sc.AddScoped<IEventValidator<E>>(sp =>
		//		new EventValidator<E>(sp, Validator)
		//		);
		//	return sc;
		//}

		//public static IServiceCollection AddEventValidator<I0, I1, I2, I3, E>(
		//	this IServiceCollection sc,
		//	Func<I0, I1, I2, I3, E, Task<bool>> Validator)
		//	=> sc.AddScoped<IEventValidator<E>>(
		//		sp =>
		//			 new EventValidator<E>(
		//				 sp,
		//				(isp, e) =>
		//					isp.Invoke<I0, I1, I2, I3, Task<bool>>((i0,i1,i2,i3) => Validator(i0,i1,i2,i3, e))
		//			 )
		//		);
		//public static IServiceCollection AddEventValidator<I0, I1, I2, E>(
		//	this IServiceCollection sc,
		//	Func<I0, I1, I2,  E, Task<bool>> Validator)
		//	=> sc.AddScoped<IEventValidator<E>>(
		//		sp =>
		//			 new EventValidator<E>(
		//				 sp,
		//				(isp, e) =>
		//					isp.Invoke<I0, I1, I2,  Task<bool>>((i0, i1, i2) => Validator(i0, i1, i2,  e))
		//			 )
		//		);
		//public static IServiceCollection AddEventValidator<I0, I1,  E>(
		//	this IServiceCollection sc,
		//	Func<I0, I1,  E, Task<bool>> Validator)
		//	=> sc.AddScoped<IEventValidator<E>>(
		//		sp =>
		//			 new EventValidator<E>(
		//				 sp,
		//				(isp, e) =>
		//					isp.Invoke<I0, I1, Task<bool>>((i0, i1) => Validator(i0, i1, e))
		//			 )
		//		);
		//public static IServiceCollection AddEventValidator<I0, E>(
		//	this IServiceCollection sc,
		//	Func<I0,  E, Task<bool>> Validator)
		//	=> sc.AddScoped<IEventValidator<E>>(
		//		sp =>
		//			 new EventValidator<E>(
		//				 sp,
		//				(isp, e) =>
		//					isp.Invoke<I0,  Task<bool>>((i0) => Validator(i0, e))
		//			 )
		//		);

	}
}
