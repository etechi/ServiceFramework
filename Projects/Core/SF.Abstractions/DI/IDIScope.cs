using System;
using System.Collections.Generic;
using System.Text;

namespace SF.DI
{
	public interface IDIScope : 
		IDisposable
	{
		IServiceProvider ServiceProvider { get; }
	}
	public interface IDIScopeFactory
	{
		IDIScope CreateScope();
	}
}
