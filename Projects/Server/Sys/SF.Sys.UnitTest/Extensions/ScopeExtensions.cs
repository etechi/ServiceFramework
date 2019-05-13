using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys;
using SF.Sys.Entities;
using SF.Sys.Clients;
using SF.Sys.UnitTest;
using SF.Sys.ServiceFeatures;
using System.Collections.Generic;

namespace SF.UT
{
	public static class ScopeExtension 
    {
		public static IScope<IServiceProvider> InitServices(
			this IScope<IServiceProvider> scope,
            IReadOnlyDictionary<string, string> args = null
            )
		{
                return from sp in scope
                       from re1 in sp.InitServices("service", args)
                       select sp;
		}
        public static IScope<IServiceProvider> InitData(
            this IScope<IServiceProvider> scope,
            IReadOnlyDictionary<string,string> args=null
            )
        {
            var nargs = new Dictionary<string, string>()
            {
                {"unittest","true"}
            };
            if (args != null)
                foreach (var p in args)
                    nargs[p.Key] = p.Value;

            return from sp in scope
                    from re2 in sp.InitServices("data", nargs)
                    select sp;
        }
    }
}
