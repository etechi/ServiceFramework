using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Annotations
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public class TypeDisplay : Attribute
	{
		public string Description { get; set; }
		public string GroupName { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }
		public string ShortName { get; set; }
	}

}
