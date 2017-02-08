using SF.Core.DI;
using SF.Metadata;
using SF.Services.ManagedServices;
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

namespace SF.AspNet.NetworkService
{	
	partial class ServiceActionDescriptor : HttpActionDescriptor
	{
		public class ParameterDescriptor : ReflectedHttpParameterDescriptor
		{
			bool FromBody { get; }
			public ParameterDescriptor(
				HttpActionDescriptor actionDescriptor, 
				ParameterInfo parameterInfo,
				bool FromBody
				) : 
				base(actionDescriptor, parameterInfo)
			{
				this.FromBody = FromBody;
			}
			public override Collection<TAttribute> GetCustomAttributes<TAttribute>()
			{
				var re=base.GetCustomAttributes<TAttribute>();
				if (typeof(TAttribute) == typeof(FromBodyAttribute) && re.Count == 0 && FromBody)
						return new Collection<TAttribute>(
							new[] { (TAttribute)(object)new FromBodyAttribute() }
							);
				return re;
			}
		}
	}
	
}
