using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys;
using SF.Sys.Entities;
using SF.Sys.Clients;
using SF.Sys.UnitTest;
using SF.Sys.ServiceFeatures;
namespace SF.UT
{
	public static class ScopeExtension 
    {
		public static IScope<IServiceProvider> InitServices(
			this IScope<IServiceProvider> scope
			)
		{
			return from sp in scope
				   from re in sp.InitServices("service")
					select sp;
		}
    }
}
