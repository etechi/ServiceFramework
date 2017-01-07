using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Annotations
{
	[AttributeUsage(AttributeTargets.Interface)]
	public class NetworkServiceAttribute : Attribute
	{
		public string Name { get; set; }
	}
	
}
