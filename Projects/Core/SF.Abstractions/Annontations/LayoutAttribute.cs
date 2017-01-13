using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Annotations
{
	[AttributeUsage(AttributeTargets.Property)]
	public class LayoutAttribute : Attribute
	{
		public int[] Positions { get; }
		public LayoutAttribute(params int[] Positions)
		{
			this.Positions = Positions;
		}
	}

}
