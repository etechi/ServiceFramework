using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public class EntityManagerAttribute : Attribute
	{
		public string Title { get; set; }
		public string FontIcon { get; set; }
	}


}
