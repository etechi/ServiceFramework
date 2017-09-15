using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.ServiceFeatures
{	
	public interface IServiceBootable 
	{
		Task<IDisposable> Boot();
	}
}
