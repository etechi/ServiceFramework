using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.All)]
	public class CategoryAttribute : Attribute
	{
		public string[] Names { get; }
		public CategoryAttribute(params string[] Names)
		{
			this.Names = Names;
		}
	}
	
}
