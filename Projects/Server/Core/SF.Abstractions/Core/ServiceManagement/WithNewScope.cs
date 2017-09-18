using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public delegate Task<R> WithNewScope<I1, I2, I3, I4, I5, I6, I7, I8, R>(Func<I1, I2, I3, I4, I5, I6, I7, I8, Task<R>> Callback);
	public delegate Task<R> WithNewScope<I1, I2, I3, I4, I5, I6, I7, R>(Func<I1, I2, I3, I4, I5, I6, I7, Task<R>> Callback);
	public delegate Task<R> WithNewScope<I1, I2, I3, I4, I5, I6, R>(Func<I1, I2, I3, I4, I5, I6, Task<R>> Callback);
	public delegate Task<R> WithNewScope<I1, I2, I3, I4, I5, R>(Func<I1, I2, I3, I4, I5, Task<R>> Callback);
	public delegate Task<R> WithNewScope<I1, I2, I3, I4, R>(Func<I1, I2, I3, I4, Task<R>> Callback);
	public delegate Task<R> WithNewScope<I1, I2, I3, R>(Func<I1, I2, I3, Task<R>> Callback);
	public delegate Task<R> WithNewScope<I1, I2, R>(Func<I1, I2, Task<R>> Callback);
	public delegate Task<R> WithNewScope<I1, R>(Func<I1, Task<R>> Callback);

}
