using SF.Core.DI;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Dispatcher;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using SF.Services.NetworkService;

namespace SF.AspNet.NetworkService
{
	class ServiceApiControllerTypeResolver : DefaultHttpControllerTypeResolver
	{
		public string Prefix { get; }
		public IEnumerable<Type> Types { get; }
		public IServiceBuildRuleProvider ServiceBuildRuleProvider { get; }
		public ServiceApiControllerTypeResolver(string Prefix, IEnumerable<Type> Types, IServiceBuildRuleProvider ServiceBuildRuleProvider) 
		{
			this.Prefix = Prefix;
			this.Types = Types;
			this.ServiceBuildRuleProvider = ServiceBuildRuleProvider;
		}

		public override ICollection<Type> GetControllerTypes(IAssembliesResolver assembliesResolver)
		{
			var re = new List<Type>(base.GetControllerTypes(assembliesResolver));
			re.AddRange(Types);
			return re;
		}
	}
	
}
