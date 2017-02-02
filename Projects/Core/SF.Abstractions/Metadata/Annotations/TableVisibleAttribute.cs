using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Metadata
{
	[AttributeUsage(AttributeTargets.Property)]
	public class TableVisibleAttribute : Attribute
	{
		public int Order { get; }
		public TableVisibleAttribute(int Order = 0)
		{
			this.Order = Order;
		}
	}

}
