using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.ServiceFeatures
{	
	public interface IServiceInitializable
	{
		string Title { get; }
		string Group { get; }
		Task Init(IServiceProvider ServiceProvider);
		int Priority { get; }
	}
}
