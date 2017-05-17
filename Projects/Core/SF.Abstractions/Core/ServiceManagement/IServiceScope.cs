using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.ServiceManagement
{
	public interface IServiceScope : 
		IDisposable
	{
		IServiceResolver ServiceResolver { get; }
	}
}
