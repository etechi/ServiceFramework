using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Hosting
{
	
	//public interface IAppInstance
	//{

	//}
	//public interface IAppTemplate
	//{

	//}
	//public interface IAppTemplateManager
	//{
	//	IAppTemplate Register(
	//		string Name,
	//		Func<IServiceProvider, long, IAppInstance> CreateAppInstance
	//		);
	//}

	//public interface IAppHost
	//{

	//}

	//public interface IAppHostManager
	//{

	//}


    public interface IAppInstance : IDisposable
    {
		IServiceProvider ServiceProvider { get; }
		EnvironmentType EnvType { get; }
		string Name { get; }
	}
	public interface IAppInstanceBuilder
	{
		IAppInstance Build();
	}
}
