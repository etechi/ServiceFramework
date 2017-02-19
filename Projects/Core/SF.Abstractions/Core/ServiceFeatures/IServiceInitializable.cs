using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.ServiceFeatures
{	
	public interface IServiceInitializable
	{
		Task Init();
	}
}
