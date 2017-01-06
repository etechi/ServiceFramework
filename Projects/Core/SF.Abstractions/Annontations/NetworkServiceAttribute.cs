using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Annotations
{
	[AttributeUsage(AttributeTargets.Interface)]
	public class NetworkServiceAttribute : Attribute
	{
		public string InterfaceName { get; set; }
	}
	
}
