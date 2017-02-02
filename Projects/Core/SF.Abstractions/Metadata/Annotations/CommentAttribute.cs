using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Field)]
	public class CommentAttribute : Attribute
	{
		public string Description { get; set; }
		public string GroupName { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }
		public string ShortName { get; set; }
	}

}
