using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.ServiceManagement
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]	
	public class FromServiceAttribute:Attribute
	{
		public Type ServiceType { get; }
		public FromServiceAttribute(Type ServiceType)
		{
			this.ServiceType = ServiceType;
		}
	}
}
