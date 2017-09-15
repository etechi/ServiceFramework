using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using SF.Core.Serialization;
using SF.Auth;
using SF.Metadata;

namespace SF.Core.NetworkService
{
	public class ServiceTypeCollection : IServiceTypeCollection
    {
		public System.Type[] Types { get; }
		public ServiceTypeCollection(Type[] Types)
		{
			this.Types = Types;

		}
		
		
	}

	

}
